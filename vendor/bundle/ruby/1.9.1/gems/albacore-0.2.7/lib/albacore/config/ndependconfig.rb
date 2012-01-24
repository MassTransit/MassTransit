require 'ostruct'
require 'albacore/support/openstruct'

module Configuration
  module NDepend
    include Albacore::Configuration

    def ndepend
      @ndependconfig ||= OpenStruct.new.extend(OpenStructToHash)
      yield(@ndependconfig) if block_given?
      @ndependconfig
    end
  end
end

