require 'albacore/ncoverreports/reportfilterbase'

module NCover
  class MethodFilter < NCover::ReportFilterBase
    def initialize(params={})
      super("Method", params)
    end    
  end
end
