module Zip
  class Inflater < Decompressor  #:nodoc:all
    def initialize(inputStream)
      super
      @zlibInflater = Zlib::Inflate.new(-Zlib::MAX_WBITS)
      @outputBuffer=""
      @hasReturnedEmptyString = ! EMPTY_FILE_RETURNS_EMPTY_STRING_FIRST
    end

    def sysread(numberOfBytes = nil, buf = nil)
      readEverything = numberOfBytes.nil?
      while (readEverything || @outputBuffer.bytesize < numberOfBytes)
        break if internal_input_finished?
        @outputBuffer << internal_produce_input(buf)
      end
      return value_when_finished if @outputBuffer.bytesize == 0 && input_finished?
      endIndex = numberOfBytes.nil? ? @outputBuffer.bytesize : numberOfBytes
      return @outputBuffer.slice!(0...endIndex)
    end

    def produce_input
      if (@outputBuffer.empty?)
        return internal_produce_input
      else
        return @outputBuffer.slice!(0...(@outputBuffer.length))
      end
    end

    # to be used with produce_input, not read (as read may still have more data cached)
    # is data cached anywhere other than @outputBuffer?  the comment above may be wrong
    def input_finished?
      @outputBuffer.empty? && internal_input_finished?
    end
    alias :eof :input_finished?
    alias :eof? :input_finished?

    private

    def internal_produce_input(buf = nil)
      retried = 0
      begin
        @zlibInflater.inflate(@inputStream.read(Decompressor::CHUNK_SIZE, buf))
      rescue Zlib::BufError
        raise if (retried >= 5) # how many times should we retry?
        retried += 1
        retry
      end
    end

    def internal_input_finished?
      @zlibInflater.finished?
    end

    # TODO: Specialize to handle different behaviour in ruby > 1.7.0 ?
    def value_when_finished   # mimic behaviour of ruby File object.
      return if @hasReturnedEmptyString
      @hasReturnedEmptyString = true
      return ""
    end
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
