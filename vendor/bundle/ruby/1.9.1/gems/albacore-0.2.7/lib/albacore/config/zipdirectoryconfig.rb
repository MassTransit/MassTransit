require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module Zip
    include Albacore::Configuration

    def zip
      @zipconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@zipconfig) if block_given?
      @zipconfig
    end
  end
end

