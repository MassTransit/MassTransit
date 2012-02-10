module Zip
  # ZipOutputStream is the basic class for writing zip files. It is
  # possible to create a ZipOutputStream object directly, passing
  # the zip file name to the constructor, but more often than not
  # the ZipOutputStream will be obtained from a ZipFile (perhaps using the
  # ZipFileSystem interface) object for a particular entry in the zip
  # archive.
  #
  # A ZipOutputStream inherits IOExtras::AbstractOutputStream in order
  # to provide an IO-like interface for writing to a single zip
  # entry. Beyond methods for mimicking an IO-object it contains
  # the method put_next_entry that closes the current entry
  # and creates a new.
  #
  # Please refer to ZipInputStream for example code.
  #
  # java.util.zip.ZipOutputStream is the original inspiration for this
  # class.

  class ZipOutputStream
    include IOExtras::AbstractOutputStream

    attr_accessor :comment

    # Opens the indicated zip file. If a file with that name already
    # exists it will be overwritten.
    def initialize(fileName, stream=false)
      super()
      @fileName = fileName
      if stream
        @outputStream = StringIO.new
      else
        @outputStream = File.new(@fileName, "wb")
      end
      @entrySet = ZipEntrySet.new
      @compressor = NullCompressor.instance
      @closed = false
      @currentEntry = nil
      @comment = nil
    end

    # Same as #initialize but if a block is passed the opened
    # stream is passed to the block and closed when the block
    # returns.
    def ZipOutputStream.open(fileName)
      return new(fileName) unless block_given?
      zos = new(fileName)
      yield zos
    ensure
      zos.close if zos
    end

	# Same as #open but writes to a filestream instead
    def ZipOutputStream.write_buffer
      zos = new('', true)
      yield zos
      return zos.close_buffer
    end

    # Closes the stream and writes the central directory to the zip file
    def close
      return if @closed
      finalize_current_entry
      update_local_headers
      write_central_directory
      @outputStream.close
      @closed = true
    end

    # Closes the stream and writes the central directory to the zip file
    def close_buffer
      return @outputStream if @closed
      finalize_current_entry
      update_local_headers
      write_central_directory
      @closed = true
      return @outputStream
    end

	  # Closes the current entry and opens a new for writing.
    # +entry+ can be a ZipEntry object or a string.
    def put_next_entry(entryname, comment = nil, extra = nil, compression_method = ZipEntry::DEFLATED,  level = Zlib::DEFAULT_COMPRESSION)
      raise ZipError, "zip stream is closed" if @closed
      new_entry = ZipEntry.new(@fileName, entryname.to_s)
      new_entry.unix_perms = entryname.unix_perms if entryname.respond_to? :unix_perms
      new_entry.comment = comment if !comment.nil?
      if (!extra.nil?)
        new_entry.extra = ZipExtraField === extra ? extra : ZipExtraField.new(extra.to_s)
      end
      new_entry.compression_method = compression_method
      init_next_entry(new_entry, level)
      @currentEntry = new_entry
    end

    def copy_raw_entry(entry)
      entry = entry.dup
      raise ZipError, "zip stream is closed" if @closed
      raise ZipError, "entry is not a ZipEntry" if !entry.kind_of?(ZipEntry)
      finalize_current_entry
      @entrySet << entry
      src_pos = entry.local_entry_offset
      entry.write_local_entry(@outputStream)
      @compressor = NullCompressor.instance
      entry.get_raw_input_stream {
        |is|
        is.seek(src_pos, IO::SEEK_SET)
        IOExtras.copy_stream_n(@outputStream, is, entry.compressed_size)
      }
      @compressor = NullCompressor.instance
      @currentEntry = nil
    end

    private
    def finalize_current_entry
      return unless @currentEntry
      finish
      @currentEntry.compressed_size = @outputStream.tell - @currentEntry.localHeaderOffset -
	    @currentEntry.calculate_local_header_size
      @currentEntry.size = @compressor.size
      @currentEntry.crc = @compressor.crc
      @currentEntry = nil
      @compressor = NullCompressor.instance
    end

    def init_next_entry(entry, level = Zlib::DEFAULT_COMPRESSION)
      finalize_current_entry
      @entrySet << entry
      entry.write_local_entry(@outputStream)
      @compressor = get_compressor(entry, level)
    end

    def get_compressor(entry, level)
      case entry.compression_method
        when ZipEntry::DEFLATED then Deflater.new(@outputStream, level)
        when ZipEntry::STORED   then PassThruCompressor.new(@outputStream)
      else raise ZipCompressionMethodError,
        "Invalid compression method: '#{entry.compression_method}'"
      end
    end

    def update_local_headers
      pos = @outputStream.tell
      @entrySet.each {
        |entry|
        @outputStream.pos = entry.localHeaderOffset
        entry.write_local_entry(@outputStream)
      }
      @outputStream.pos = pos
    end

    def write_central_directory
      cdir = ZipCentralDirectory.new(@entrySet, @comment)
      cdir.write_to_stream(@outputStream)
    end

    protected

    def finish
      @compressor.finish
    end

    public
    # Modeled after IO.<<
    def << (data)
      @compressor << data
    end
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
