require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module Docu
    include Albacore::Configuration
    
    def self.docuconfig
      @docuconfig ||= OpenStruct.new.extend(OpenStructToHash)
    end

    def docu
      config = Docu.docuconfig
      yield(config) if block_given?
      config
    end

    def self.included(obj)
      docuconfig.command = "Docu.exe"
    end
  end
end

