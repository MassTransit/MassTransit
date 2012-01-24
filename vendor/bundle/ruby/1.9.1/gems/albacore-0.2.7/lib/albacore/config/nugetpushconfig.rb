require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NuGetPush
    include Albacore::Configuration

    def self.nugetpushconfig
      @config ||= OpenStruct.new.extend(OpenStructToHash).extend(NuGetPush)
    end

    def nugetpush
      @config ||= NuGetPush.nugetpushconfig
      yield(@config) if block_given?
      @config
    end
    
  end
end
