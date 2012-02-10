require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NCoverReport
    include Albacore::Configuration

    def ncoverreport
      @ncoverreportconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@ncoverreportconfig) if block_given?
      @ncoverreportconfig
    end
  end
end

