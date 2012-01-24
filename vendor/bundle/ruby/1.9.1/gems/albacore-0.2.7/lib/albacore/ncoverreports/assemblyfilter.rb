require 'albacore/ncoverreports/reportfilterbase'

module NCover
  class AssemblyFilter < NCover::ReportFilterBase
    def initialize(params={})
      super("Assembly", params)
    end    
  end
end  
  