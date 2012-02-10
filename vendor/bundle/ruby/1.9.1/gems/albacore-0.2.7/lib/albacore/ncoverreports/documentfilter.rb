require 'albacore/ncoverreports/reportfilterbase'

module NCover
  class DocumentFilter < NCover::ReportFilterBase
    def initialize(params={})
      super("Document", params)
    end    
  end
end
