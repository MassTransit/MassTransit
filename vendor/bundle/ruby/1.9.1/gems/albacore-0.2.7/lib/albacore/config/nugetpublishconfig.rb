require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NuGetPublish
    include Albacore::Configuration

    def self.nugetpublishconfig
      @config ||= OpenStruct.new.extend(OpenStructToHash).extend(NuGetPublish)
    end

    def nugetpublish
      @config ||= NuGetPublish.nugetpublishconfig
      yield(@config) if block_given?
      @config
    end
    
  end
end
