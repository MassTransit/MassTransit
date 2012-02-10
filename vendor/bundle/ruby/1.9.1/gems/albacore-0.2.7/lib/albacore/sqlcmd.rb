require 'albacore/albacoretask'

class SQLCmd
  include Albacore::Task
  include Albacore::RunCommand
  
  attr_accessor :server, :database, :username, :password, :trusted_connection, :batch_abort, :severity
  attr_array :scripts
  attr_hash :variables
  
  def initialize
    @require_valid_command = false
    @scripts=[]
    @variables={}
    @trusted_connection = true
    @batch_abort = true
    @severity = nil
    super()
    update_attributes Albacore.configuration.sqlcmd.to_hash
  end
  
  def execute
    return unless check_command
    
    cmd_params=[]
    serverParam = @server.nil? ? build_parameter("S", ".") : build_parameter("S", @server)
    cmd_params << serverParam
    cmd_params << build_parameter("d", @database) unless @database.nil?
    cmd_params << get_authentication_params
    cmd_params << build_variable_list if @variables.length > 0
    cmd_params << get_batch_abort_param
    cmd_params << build_script_list if @scripts.length > 0
    cmd_params << build_parameter("V", @severity) unless @severity.nil?
    
    result = run_command "SQLCmd", cmd_params.join(" ")
    
    failure_msg = 'SQLCmd Failed. See Build Log For Detail.'
    fail_with_message failure_msg if !result
  end

  def get_batch_abort_param
    "-b" if (@scripts.length > 1 && @batch_abort)
  end

  def get_authentication_params
    integratedParam = "-E" if @trusted_connection
    if ((!(@username.nil?)) and (!(@password.nil?)))
      integratedParam = build_parameter("U", @username) + " " + build_parameter("P", @password)
    end
    integratedParam
  end

  def check_command
    sql2008cmdPath = File.join(ENV['SystemDrive'],'program files','microsoft sql server','100','tools','binn', 'sqlcmd.exe')
    sql2005cmdPath = File.join(ENV['SystemDrive'],'program files','microsoft sql server','90','tools','binn', 'sqlcmd.exe')
    sql2008cmdPathx86 = File.join(ENV['SystemDrive'],'program files (x86)','microsoft sql server','100','tools','binn', 'sqlcmd.exe')
    sql2005cmdPathx86 = File.join(ENV['SystemDrive'],'program files (x86)','microsoft sql server','90','tools','binn', 'sqlcmd.exe')
    @command = [sql2008cmdPath, sql2005cmdPath, sql2008cmdPathx86, sql2005cmdPathx86].select { |p| File.exist?(p) }.first
    return true if @command != nil
    fail_with_message 'SQLCmd.command cannot be nil.'
    return false
  end

  def build_script_list
    @scripts.map{|s| "-i \"#{s.strip}\""}.join(" ")
  end
  
  def build_parameter(param_name, param_value)
    "-#{param_name} \"#{param_value}\""
  end
  
  def build_variable_list
    vars = []
    @variables.each do |k,v| 
      vars << "-v #{k}=#{v}"
    end
    vars.join(" ")
  end
  
end
