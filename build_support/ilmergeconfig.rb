require 'ostruct'
require 'albacore/config/netversion'
require 'albacore/support/openstruct'

module Configuration
  module ILMerge
    include Configuration::NetVersion
    include Albacore::Configuration

    def self.ilmergeconfig
      @config ||= OpenStruct.new.extend(OpenStructToHash).extend(ILMerge)
    end

    def ilmerge
      @config ||= ILMerge.ilmergeconfig
      yield(@config) if block_given?
      @config
    end

    def self.included(mod)
      self.ilmergeconfig.command = File.join('lib', 'ilmerge', "ilmerge.exe")
    end
  end
end