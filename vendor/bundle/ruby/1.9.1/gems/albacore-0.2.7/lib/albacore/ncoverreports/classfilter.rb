require 'albacore/ncoverreports/reportfilterbase'

module NCover
  class ClassFilter < NCover::ReportFilterBase
    def initialize(params={})
      super("Class", params)
    end    
  end
end
