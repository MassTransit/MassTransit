require 'albacore/albacoretask'

class XUnitTestRunner
  TaskName = :xunit
  include Albacore::Task
  include Albacore::RunCommand

  attr_accessor :html_output, :skip_test_fail
  attr_array :options,:assembly,:assemblies

  def initialize(command=nil)
    @options=[]
    super()
    update_attributes Albacore.configuration.xunit.to_hash
    @command = command unless command.nil?
  end

  def get_command_line
    command_params = []
    command_params << @command
    command_params << get_command_parameters
    commandline = command_params.join(" ")
    @logger.debug "Build XUnit Test Runner Command Line: " + commandline
    commandline
  end
  
  def get_command_parameters
    command_params = []	
    command_params << @options.join(" ") unless @options.nil?
    command_params << build_html_output unless @html_output.nil?
    command_params
  end

  def execute()    		
    @assemblies = [] if @assemblies.nil?
    @assemblies << @assembly unless @assembly.nil?
    fail_with_message 'At least one assembly is required for assemblies attr' if @assemblies.length==0	
    failure_message = 'XUnit Failed. See Build Log For Detail'		
    
    @assemblies.each do |assm|
      command_params = get_command_parameters.collect{ |p| p % File.basename(assm) }
      command_params.insert(0,assm)	
      result = run_command "XUnit", command_params.join(" ")
      fail_with_message failure_message if !result && (!@skip_test_fail || $?.exitstatus > 1)
    end       
  end

  def build_html_output			
    fail_with_message 'Directory is required for html_output' if !File.directory?(File.expand_path(@html_output))
    "/html \"#{File.join(File.expand_path(@html_output),"%s.html")}\""
  end
end
