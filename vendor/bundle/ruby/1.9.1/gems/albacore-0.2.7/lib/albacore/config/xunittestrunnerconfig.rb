require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module XUnit
    include Albacore::Configuration

    def xunit
      @xunitconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@xunitconfig) if block_given?
      @xunitconfig
    end
  end
end

