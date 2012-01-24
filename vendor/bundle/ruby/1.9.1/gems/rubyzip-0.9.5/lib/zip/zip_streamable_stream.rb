module Zip
  class ZipStreamableStream < DelegateClass(ZipEntry) #nodoc:all
    def initialize(entry)
      super(entry)
      @tempFile = Tempfile.new(File.basename(name), File.dirname(zipfile))
      @tempFile.binmode
    end

    def get_output_stream
      if block_given?
        begin
          yield(@tempFile)
        ensure
          @tempFile.close
        end
      else
        @tempFile
      end
    end

    def get_input_stream
      if ! @tempFile.closed?
        raise StandardError, "cannot open entry for reading while its open for writing - #{name}"
      end
      @tempFile.open # reopens tempfile from top
      @tempFile.binmode
      if block_given?
        begin
          yield(@tempFile)
        ensure
          @tempFile.close
        end
      else
        @tempFile
      end
    end
    
    def write_to_zip_output_stream(aZipOutputStream)
      aZipOutputStream.put_next_entry(self)
      get_input_stream { |is| IOExtras.copy_stream(aZipOutputStream, is) } 
    end
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
