require 'delegate'
require 'iconv' unless RUBY_VERSION >= '1.9'
require 'singleton'
require 'tempfile'
require 'fileutils'
require 'stringio'
require 'zlib'
require 'zip/stdrubyext'
require 'zip/ioextras'
require 'rbconfig'

require 'zip/zip_entry'
require 'zip/zip_extra_field'
require 'zip/zip_entry_set'
require 'zip/zip_central_directory'
require 'zip/zip_file'
require 'zip/zip_input_stream'
require 'zip/zip_output_stream'
require 'zip/decompressor'
require 'zip/compressor'
require 'zip/null_decompressor'
require 'zip/null_compressor'
require 'zip/pass_thru_compressor'
require 'zip/pass_thru_decompressor'
require 'zip/inflater'
require 'zip/deflater'
require 'zip/zip_streamable_stream'
require 'zip/zip_streamable_directory'
require 'zip/constants'

if Tempfile.superclass == SimpleDelegator
  require 'zip/tempfile_bugfixed'
  Tempfile = BugFix::Tempfile
end

module Zlib  #:nodoc:all
  if ! const_defined? :MAX_WBITS
    MAX_WBITS = Zlib::Deflate.MAX_WBITS
  end
end

module Zip
  class ZipError < StandardError ; end

  class ZipEntryExistsError            < ZipError; end
  class ZipDestinationFileExistsError  < ZipError; end
  class ZipCompressionMethodError      < ZipError; end
  class ZipEntryNameError              < ZipError; end
  class ZipInternalError               < ZipError; end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
