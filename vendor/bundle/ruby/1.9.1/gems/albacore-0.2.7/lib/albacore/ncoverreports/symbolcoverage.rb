require 'albacore/ncoverreports/codecoveragebase'

module NCover
  class SymbolCoverage < NCover::CodeCoverageBase
    def initialize(params={})
      super("SymbolCoverage", params)
    end    
  end
end
