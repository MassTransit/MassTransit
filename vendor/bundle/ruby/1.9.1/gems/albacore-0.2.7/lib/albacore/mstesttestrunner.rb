require 'albacore/albacoretask'

class MSTestTestRunner
  TaskName = :mstest
  include Albacore::Task
  include Albacore::RunCommand
  
  attr_array :assemblies, :tests, :options
  
  def initialize(command=nil)
    @options=[]
    @assemblies=[]
    @tests=[]
    super()
    update_attributes Albacore.configuration.mstest.to_hash
    @command = command unless command.nil?
  end
  
  def get_command_line
    command_params = []
    command_params << @command
    command_params << get_command_parameters
    commandline = command_params.join(" ")
    @logger.debug "Build MSTest Test Runner Command Line: " + commandline
    commandline
  end
  
  def get_command_parameters
    command_params = []
    command_params << @options.join(" ") unless @options.nil?
    command_params << @assemblies.map{|asm| "/testcontainer:\"#{asm}\""}.join(' ') unless @assemblies.nil?
    command_params << @tests.map{|test| "/test:#{test}"}.join(' ') unless @tests.nil?
    command_params
  end
  
  def execute()
    command_params = get_command_parameters
    result = run_command "MSTest", command_params.join(" ")
    
    failure_message = 'MSTest Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end  
end
