module NCover
  class FullCoverageReport
    attr_accessor :output_path
    
    def report_type
      :FullCoverageReport
    end
    
    def report_format
      :Html
    end
  end
end
