require 'albacore/ncoverreports/codecoveragebase'

module NCover
  class MethodCoverage < NCover::CodeCoverageBase
    def initialize(params={})
      super("MethodCoverage", params)
    end    
  end
end  
  