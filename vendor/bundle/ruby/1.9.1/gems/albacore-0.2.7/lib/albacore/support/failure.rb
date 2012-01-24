require 'albacore/support/logging'

module Failure
  include Logging
  
  def initialize
    super()
  end
  
  def fail_with_message(msg)
    @logger.fatal msg
    fail msg
  end
end

