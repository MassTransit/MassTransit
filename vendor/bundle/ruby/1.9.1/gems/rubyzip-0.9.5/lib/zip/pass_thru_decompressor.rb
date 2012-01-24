module Zip
  class PassThruDecompressor < Decompressor  #:nodoc:all
    def initialize(inputStream, charsToRead)
      super inputStream
      @charsToRead = charsToRead
      @readSoFar = 0
      @hasReturnedEmptyString = ! EMPTY_FILE_RETURNS_EMPTY_STRING_FIRST
    end

    # TODO: Specialize to handle different behaviour in ruby > 1.7.0 ?
    def sysread(numberOfBytes = nil, buf = nil)
      if input_finished?
        hasReturnedEmptyStringVal = @hasReturnedEmptyString
        @hasReturnedEmptyString = true
        return "" unless hasReturnedEmptyStringVal
        return
      end

      if (numberOfBytes == nil || @readSoFar + numberOfBytes > @charsToRead)
        numberOfBytes = @charsToRead - @readSoFar
      end
      @readSoFar += numberOfBytes
      @inputStream.read(numberOfBytes, buf)
    end

    def produce_input
      sysread(Decompressor::CHUNK_SIZE)
    end

    def input_finished?
      (@readSoFar >= @charsToRead)
    end
    alias :eof :input_finished?
    alias :eof? :input_finished?
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
