module Zip
  # ZipInputStream is the basic class for reading zip entries in a 
  # zip file. It is possible to create a ZipInputStream object directly, 
  # passing the zip file name to the constructor, but more often than not 
  # the ZipInputStream will be obtained from a ZipFile (perhaps using the 
  # ZipFileSystem interface) object for a particular entry in the zip 
  # archive.
  #
  # A ZipInputStream inherits IOExtras::AbstractInputStream in order
  # to provide an IO-like interface for reading from a single zip 
  # entry. Beyond methods for mimicking an IO-object it contains 
  # the method get_next_entry for iterating through the entries of 
  # an archive. get_next_entry returns a ZipEntry object that describes
  # the zip entry the ZipInputStream is currently reading from.
  #
  # Example that creates a zip archive with ZipOutputStream and reads it 
  # back again with a ZipInputStream.
  #
  #   require 'zip/zip'
  #   
  #   Zip::ZipOutputStream::open("my.zip") { 
  #     |io|
  #   
  #     io.put_next_entry("first_entry.txt")
  #     io.write "Hello world!"
  #   
  #     io.put_next_entry("adir/first_entry.txt")
  #     io.write "Hello again!"
  #   }
  #
  #   
  #   Zip::ZipInputStream::open("my.zip") {
  #     |io|
  #   
  #     while (entry = io.get_next_entry)
  #       puts "Contents of #{entry.name}: '#{io.read}'"
  #     end
  #   }
  #
  # java.util.zip.ZipInputStream is the original inspiration for this 
  # class.

  class ZipInputStream 
    include IOExtras::AbstractInputStream

    # Opens the indicated zip file. An exception is thrown
    # if the specified offset in the specified filename is
    # not a local zip entry header.
    def initialize(filename, offset = 0)
      super()
      @archiveIO = File.open(filename, "rb")
      @archiveIO.seek(offset, IO::SEEK_SET)
      @decompressor = NullDecompressor.instance
      @currentEntry = nil
    end
    
    def close
      @archiveIO.close
    end

    # Same as #initialize but if a block is passed the opened
    # stream is passed to the block and closed when the block
    # returns.    
    def ZipInputStream.open(filename)
      return new(filename) unless block_given?
      
      zio = new(filename)
      yield zio
    ensure
      zio.close if zio
    end

    # Returns a ZipEntry object. It is necessary to call this
    # method on a newly created ZipInputStream before reading from 
    # the first entry in the archive. Returns nil when there are 
    # no more entries.

    def get_next_entry
      @archiveIO.seek(@currentEntry.next_header_offset, 
                      IO::SEEK_SET) if @currentEntry
      open_entry
    end

    # Rewinds the stream to the beginning of the current entry
    def rewind
      return if @currentEntry.nil?
      @lineno = 0
      @archiveIO.seek(@currentEntry.localHeaderOffset, 
		      IO::SEEK_SET)
      open_entry
    end

    # Modeled after IO.sysread
    def sysread(numberOfBytes = nil, buf = nil)
      @decompressor.sysread(numberOfBytes, buf)
    end

    def eof
      @outputBuffer.empty? && @decompressor.eof
    end
    alias :eof? :eof

    protected

    def open_entry
      @currentEntry = ZipEntry.read_local_entry(@archiveIO)
      if (@currentEntry == nil) 
	@decompressor = NullDecompressor.instance
      elsif @currentEntry.compression_method == ZipEntry::STORED
	@decompressor = PassThruDecompressor.new(@archiveIO, 
						 @currentEntry.size)
      elsif @currentEntry.compression_method == ZipEntry::DEFLATED
	@decompressor = Inflater.new(@archiveIO)
      else
	raise ZipCompressionMethodError,
	  "Unsupported compression method #{@currentEntry.compression_method}"
      end
      flush
      return @currentEntry
    end

    def produce_input
      @decompressor.produce_input
    end

    def input_finished?
      @decompressor.input_finished?
    end
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
