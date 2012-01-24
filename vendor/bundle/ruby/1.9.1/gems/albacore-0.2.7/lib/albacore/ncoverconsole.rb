require 'albacore/albacoretask'

class NCoverConsole
  include Albacore::Task
  include Albacore::RunCommand
  
  attr_accessor :testrunner
  attr_array :cover_assemblies, :exclude_assemblies, :coverage, :exclude_attributes
  attr_hash :output
  
  def initialize
    @register_dll = true
    @output = {}
    @cover_assemblies = []
    @exclude_assemblies = []
    @exclude_attributes = []
    @coverage = []
    super()
    update_attributes Albacore.configuration.ncoverconsole.to_hash
  end
  
  def no_registration
    @register_dll = false
  end
  
  def execute
    return unless check_for_testrunner
    
    command_parameters = []
    command_parameters << "//reg" if @register_dll
    command_parameters << build_output_options(@output) unless @output.nil?
    command_parameters << build_parameter_list("assemblies", @cover_assemblies) unless @cover_assemblies.empty?
    command_parameters << build_parameter_list("exclude-assemblies", @exclude_assemblies) unless @exclude_assemblies.empty?
    command_parameters << build_parameter_list("exclude-attributes", @exclude_attributes) unless @exclude_attributes.empty?
    command_parameters << build_coverage_list(@coverage) unless @coverage.empty?
    command_parameters << @testrunner.get_command_line
    
    result = run_command "NCover.Console", command_parameters.join(" ")
    
    failure_msg = 'Code Coverage Analysis Failed. See Build Log For Detail.'
    fail_with_message failure_msg if !result
  end
  
  def check_for_testrunner
    return true if (!@testrunner.nil?)
    msg = 'testrunner cannot be nil.'
    @logger.info msg
    fail
    return false
  end
  
  def build_output_options(output)
    options = []
    output.each do |key, value|
      options << "//#{key} #{value}"
    end
    options.join(" ")
  end
  
  def build_parameter_list(param_name, list)
    list = list.map{|asm| "\"#{asm}\""}.join(';')
    "//#{param_name} #{list}"
  end
  
  def build_coverage_list(coverage)
    "//coverage-type \"#{coverage.join(', ')}\""
  end
end
