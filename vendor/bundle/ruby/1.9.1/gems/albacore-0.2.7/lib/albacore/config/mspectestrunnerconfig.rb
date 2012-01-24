require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module MSpec
    include Albacore::Configuration

    def mspec
      @mspecconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@mspecconfig) if block_given?
      @mspecconfig
    end
  end
end

