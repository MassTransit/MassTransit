require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module Unzip
    include Albacore::Configuration

    def unzip
      @unzipconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@unzipconfig) if block_given?
      @unzipconfig
    end
  end
end

