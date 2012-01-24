require 'albacore/albacoretask'

class NAnt 
  include Albacore::Task
  include Albacore::RunCommand
  
  attr_accessor :build_file
  attr_array :targets
  attr_hash :properties
  
  def initialize
    super()
    update_attributes Albacore.configuration.nant.to_hash
  end
  
  def execute
    command_parameters = []
    command_parameters << "-buildfile:#{@build_file}" unless @build_file.nil?
    command_parameters << "#{build_properties}" unless @properties.nil?
    command_parameters << "#{build_targets}" unless @targets.nil?
    
    result = run_command "NAnt", command_parameters.join(" ")
    
    failure_msg = 'NAnt task Failed. See Build Log For Detail.'
    fail_with_message failure_msg if !result
  end
  
  private 
  
  def build_targets
    @targets.join " "
  end
  
  def build_properties
     @properties.map {|key, value| "-D:#{key}=#{value}" }.join(" ")
  end
  
end
