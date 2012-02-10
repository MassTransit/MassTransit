require 'albacore/albacoretask'
require 'albacore/config/msbuildconfig.rb'

class MSBuild
  include Albacore::Task
  include Albacore::RunCommand
  include Configuration::MSBuild
  
  attr_accessor :solution, :verbosity, :loggermodule
  attr_array :targets
  attr_hash :properties
  
  def initialize
    super()
    update_attributes msbuild.to_hash
  end
  
  def execute
    build_solution(@solution)
  end
  
  def build_solution(solution)
    check_solution solution
    
    command_parameters = []
    command_parameters << "\"#{solution}\""
    command_parameters << "\"/verbosity:#{@verbosity}\"" if @verbosity != nil
    command_parameters << "\"/logger:#{@loggermodule}\"" if @loggermodule != nil
    command_parameters << build_properties if @properties != nil
    command_parameters << "\"/target:#{build_targets}\"" if @targets != nil
    
    result = run_command "MSBuild", command_parameters.join(" ")
    
    failure_message = 'MSBuild Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end
  
  def check_solution(file)
    return if file
    msg = 'solution cannot be nil'
    fail_with_message msg
  end
  
  def build_targets
    @targets.join ";"
  end

  def build_properties
    option_text = []
    @properties.each do |key, value|
      option_text << "/p:#{key}\=\"#{value}\""
    end
    option_text.join(" ")
  end
end
