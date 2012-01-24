require 'albacore/support/updateattributes'

module NCover
  class CodeCoverageBase
    include UpdateAttributes
    
    attr_accessor :coverage_type, :minimum, :item_type 
    
    def initialize(coverage_type, params={})
      @coverage_type = coverage_type
      @minimum = 0
      @item_type = :View
      update_attributes(params) unless params.nil?
      super()
    end
    
    def get_coverage_options
      options = "#{@coverage_type}"
      options << ":#{@minimum}" unless @minimum.nil?
      options << ":#{@item_type}" unless @item_type.nil?
      options
    end
  end
end  
  
