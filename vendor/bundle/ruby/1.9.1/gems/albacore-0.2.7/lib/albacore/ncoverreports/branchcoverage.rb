require 'albacore/ncoverreports/codecoveragebase'

module NCover
  class BranchCoverage < NCover::CodeCoverageBase
    def initialize(params={})
      super("BranchCoverage", params)
    end    
  end
end  
  