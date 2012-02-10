require 'ostruct'
require 'albacore/config/netversion'
require 'albacore/support/openstruct'

module Configuration
  module AspNetCompiler
    include Configuration::NetVersion
    include Albacore::Configuration

    def self.aspnetcompilerconfig
      @config ||= OpenStruct.new.extend(OpenStructToHash).extend(AspNetCompiler)
    end

    def aspnetcompiler
      config ||= AspNetCompiler.aspnetcompilerconfig
      yield(config) if block_given?
      config
    end

    def self.included(mod)
      self.aspnetcompilerconfig.use :net40
    end

    def use(netversion)
      netversion = :net20 if netversion == :net35 # since .net 3.5 doesn't have aspnet_compiler use .net 2.0
      self.command = File.join(get_net_version(netversion), "aspnet_compiler.exe")
    end
  end
end
