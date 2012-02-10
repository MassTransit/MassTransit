require 'albacore/albacoretask'

class Exec
  include Albacore::Task
  include Albacore::RunCommand

  def initialize
    super()
    update_attributes Albacore.configuration.exec.to_hash
  end
    
  def execute
    result = run_command "Exec"
    
    failure_message = 'Exec Failed. See Build Log For Detail'
    fail_with_message failure_message if !result
  end
end
