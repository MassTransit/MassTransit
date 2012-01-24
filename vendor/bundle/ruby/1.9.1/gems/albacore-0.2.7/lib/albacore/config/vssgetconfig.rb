require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module Vss
    include Albacore::Configuration

    def vss
      @vssconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@vssconfig) if block_given?
      @vssconfig
    end
  end
end
