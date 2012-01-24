require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NuGetPack
    include Albacore::Configuration

    def self.nugetpackconfig
      @config ||= OpenStruct.new.extend(OpenStructToHash).extend(NuGetPack)
    end

    def nugetpack
      config ||= NuGetPack.nugetpackconfig
      yield(config) if block_given?
      config
    end
    
  end
end
