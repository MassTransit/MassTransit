require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module Exec
    include Albacore::Configuration

    def exec
      @execconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@execconfig) if block_given?
      @execconfig
    end
  end
end

