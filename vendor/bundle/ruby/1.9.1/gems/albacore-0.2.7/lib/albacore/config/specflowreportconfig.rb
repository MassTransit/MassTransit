require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module SpecFlowReport
    include Albacore::Configuration

    def self.specflowreportconfig
      @specflowreportconfig ||= OpenStruct.new.extend(OpenStructToHash)
    end

    def specflowreport
      config = SpecFlowReport.specflowreportconfig
      yield(config) if block_given?
      config
    end

    def self.included(obj)
      specflowreportconfig.command = 'specflow.exe'
      specflowreportconfig.report = 'nunitexecutionreport'
    end
  end
end

