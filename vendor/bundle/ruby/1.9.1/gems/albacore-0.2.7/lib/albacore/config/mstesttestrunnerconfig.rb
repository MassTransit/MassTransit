require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module MSTest
    include Albacore::Configuration

    def mstest
      @mstestconfig ||= OpenStruct.new.extend(OpenStructToHash).extend(MSTest)
      yield(@mstestconfig) if block_given?
      @mstestconfig
    end
  end
end

