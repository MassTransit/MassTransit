require 'albacore/ncoverreports/reportfilterbase'

module NCover
  class NamespaceFilter < NCover::ReportFilterBase
    def initialize(params={})
      super("Namespace", params)
    end    
  end
end
