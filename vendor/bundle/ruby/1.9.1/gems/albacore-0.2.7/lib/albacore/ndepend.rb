require 'albacore/albacoretask'

class NDepend
  include Albacore::Task
  include Albacore::RunCommand

  attr_accessor :project_file

  def initialize()
    super()
    update_attributes Albacore.configuration.ndepend.to_hash
  end
  
  def execute
    return unless check_command
    result = run_command @command, create_parameters.join(" ")
    failure_message = 'Command Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end

  def create_parameters
    params = []
    params << File.expand_path(@project_file)
    return params
  end

  def check_command
    return true if @project_file
    fail_with_message 'A ndepend project file is required'
    return false
  end

end
