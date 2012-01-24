module Zip
  # ZipFile is modeled after java.util.zip.ZipFile from the Java SDK.
  # The most important methods are those inherited from
  # ZipCentralDirectory for accessing information about the entries in
  # the archive and methods such as get_input_stream and
  # get_output_stream for reading from and writing entries to the
  # archive. The class includes a few convenience methods such as
  # #extract for extracting entries to the filesystem, and #remove,
  # #replace, #rename and #mkdir for making simple modifications to
  # the archive.
  #
  # Modifications to a zip archive are not committed until #commit or
  # #close is called. The method #open accepts a block following
  # the pattern from File.open offering a simple way to
  # automatically close the archive when the block returns.
  #
  # The following example opens zip archive <code>my.zip</code>
  # (creating it if it doesn't exist) and adds an entry
  # <code>first.txt</code> and a directory entry <code>a_dir</code>
  # to it.
  #
  #   require 'zip/zip'
  #
  #   Zip::ZipFile.open("my.zip", Zip::ZipFile::CREATE) {
  #    |zipfile|
  #     zipfile.get_output_stream("first.txt") { |f| f.puts "Hello from ZipFile" }
  #     zipfile.mkdir("a_dir")
  #   }
  #
  # The next example reopens <code>my.zip</code> writes the contents of
  # <code>first.txt</code> to standard out and deletes the entry from
  # the archive.
  #
  #   require 'zip/zip'
  #
  #   Zip::ZipFile.open("my.zip", Zip::ZipFile::CREATE) {
  #     |zipfile|
  #     puts zipfile.read("first.txt")
  #     zipfile.remove("first.txt")
  #   }
  #
  # ZipFileSystem offers an alternative API that emulates ruby's
  # interface for accessing the filesystem, ie. the File and Dir classes.

  class ZipFile < ZipCentralDirectory

    CREATE = 1

    attr_reader :name

    # default -> false
    attr_accessor :restore_ownership
    # default -> false
    attr_accessor :restore_permissions
    # default -> true
    attr_accessor :restore_times

    # Opens a zip archive. Pass true as the second parameter to create
    # a new archive if it doesn't exist already.
    def initialize(fileName, create = nil, buffer = false)
      super()
      @name = fileName
      @comment = ""
      if (File.exists?(fileName)) and !buffer
        File.open(name, "rb") { |f| read_from_stream(f) }
      elsif (create)
        @entrySet = ZipEntrySet.new
      else
        raise ZipError, "File #{fileName} not found"
      end
      @create = create
      @storedEntries = @entrySet.dup

      @restore_ownership = false
      @restore_permissions = false
      @restore_times = true
    end

    # Same as #new. If a block is passed the ZipFile object is passed
    # to the block and is automatically closed afterwards just as with
    # ruby's builtin File.open method.
    def ZipFile.open(fileName, create = nil)
      zf = ZipFile.new(fileName, create)
      if block_given?
        begin
          yield zf
        ensure
          zf.close
        end
      else
        zf
      end
    end

    # Same as #open. But outputs data to a buffer instead of a file
    def ZipFile.add_buffer
      zf = ZipFile.new('', true, true)
      begin
        yield zf
      ensure
        buffer = zf.write_buffer
        return buffer
      end
    end

  # Returns the zip files comment, if it has one
    attr_accessor :comment

    # Iterates over the contents of the ZipFile. This is more efficient
    # than using a ZipInputStream since this methods simply iterates
    # through the entries in the central directory structure in the archive
    # whereas ZipInputStream jumps through the entire archive accessing the
    # local entry headers (which contain the same information as the
    # central directory).
    def ZipFile.foreach(aZipFileName, &block)
      ZipFile.open(aZipFileName) {
        |zipFile|
        zipFile.each(&block)
      }
    end

    # Returns an input stream to the specified entry. If a block is passed
    # the stream object is passed to the block and the stream is automatically
    # closed afterwards just as with ruby's builtin File.open method.
    def get_input_stream(entry, &aProc)
      get_entry(entry).get_input_stream(&aProc)
    end

    # Returns an output stream to the specified entry. If a block is passed
    # the stream object is passed to the block and the stream is automatically
    # closed afterwards just as with ruby's builtin File.open method.
    def get_output_stream(entry, permissionInt = nil, &aProc)
      newEntry = entry.kind_of?(ZipEntry) ? entry : ZipEntry.new(@name, entry.to_s)
      if newEntry.directory?
        raise ArgumentError,
          "cannot open stream to directory entry - '#{newEntry}'"
      end
      newEntry.unix_perms = permissionInt
      zipStreamableEntry = ZipStreamableStream.new(newEntry)
      @entrySet << zipStreamableEntry
      zipStreamableEntry.get_output_stream(&aProc)
    end

    # Returns the name of the zip archive
    def to_s
      @name
    end

    # Returns a string containing the contents of the specified entry
    def read(entry)
      get_input_stream(entry) { |is| is.read }
    end

    # Convenience method for adding the contents of a file to the archive
    def add(entry, srcPath, &continueOnExistsProc)
      continueOnExistsProc ||= proc { false }
      check_entry_exists(entry, continueOnExistsProc, "add")
      newEntry = entry.kind_of?(ZipEntry) ? entry : ZipEntry.new(@name, entry.to_s)
      newEntry.gather_fileinfo_from_srcpath(srcPath)
      @entrySet << newEntry
    end

    # Removes the specified entry.
    def remove(entry)
      @entrySet.delete(get_entry(entry))
    end

    # Renames the specified entry.
    def rename(entry, newName, &continueOnExistsProc)
      foundEntry = get_entry(entry)
      check_entry_exists(newName, continueOnExistsProc, "rename")
      @entrySet.delete(foundEntry)
      foundEntry.name = newName
      @entrySet << foundEntry
    end

    # Replaces the specified entry with the contents of srcPath (from
    # the file system).
    def replace(entry, srcPath)
      check_file(srcPath)
      add(remove(entry), srcPath)
    end

    # Extracts entry to file destPath.
    def extract(entry, destPath, &onExistsProc)
      onExistsProc ||= proc { false }
      foundEntry = get_entry(entry)
      foundEntry.extract(destPath, &onExistsProc)
    end

    # Commits changes that has been made since the previous commit to
    # the zip archive.
    def commit
      return if ! commit_required?
      on_success_replace(name) {
        |tmpFile|
        ZipOutputStream.open(tmpFile) {
          |zos|

          @entrySet.each {
            |e|
            e.write_to_zip_output_stream(zos)
            e.dirty = false
          }
          zos.comment = comment
        }
        true
      }
      initialize(name)
    end

    # Write buffer write changes to buffer and return
    def write_buffer
      buffer = ZipOutputStream.write_buffer do |zos|
        @entrySet.each { |e| e.write_to_zip_output_stream(zos) }
        zos.comment = comment
      end
      return buffer
    end

    # Closes the zip file committing any changes that has been made.
    def close
      commit
    end

    # Returns true if any changes has been made to this archive since
    # the previous commit
    def commit_required?
      @entrySet.each do |e|
        return true if e.dirty
      end
      return @entrySet != @storedEntries || @create == ZipFile::CREATE
    end

    # Searches for entry with the specified name. Returns nil if
    # no entry is found. See also get_entry
    def find_entry(entry)
      @entrySet.detect {
        |e|
        e.name.sub(/\/$/, "") == entry.to_s.sub(/\/$/, "")
      }
    end

    # Searches for an entry just as find_entry, but throws Errno::ENOENT
    # if no entry is found.
    def get_entry(entry)
      selectedEntry = find_entry(entry)
      unless selectedEntry
        raise Errno::ENOENT, entry
      end
      selectedEntry.restore_ownership = @restore_ownership
      selectedEntry.restore_permissions = @restore_permissions
      selectedEntry.restore_times = @restore_times

      return selectedEntry
    end

    # Creates a directory
    def mkdir(entryName, permissionInt = 0755)
      if find_entry(entryName)
        raise Errno::EEXIST, "File exists - #{entryName}"
      end
      @entrySet << ZipStreamableDirectory.new(@name, entryName.to_s.ensure_end("/"), nil, permissionInt)
    end

    private

    def is_directory(newEntry, srcPath)
      srcPathIsDirectory = File.directory?(srcPath)
      if newEntry.is_directory && ! srcPathIsDirectory
        raise ArgumentError,
          "entry name '#{newEntry}' indicates directory entry, but "+
          "'#{srcPath}' is not a directory"
            elsif ! newEntry.is_directory && srcPathIsDirectory
        newEntry.name += "/"
      end
      return newEntry.is_directory && srcPathIsDirectory
    end

    def check_entry_exists(entryName, continueOnExistsProc, procedureName)
      continueOnExistsProc ||= proc { false }
      if @entrySet.detect { |e| e.name == entryName }
        if continueOnExistsProc.call
          remove get_entry(entryName)
        else
          raise ZipEntryExistsError,
            procedureName+" failed. Entry #{entryName} already exists"
        end
      end
    end

    def check_file(path)
      unless File.readable? path
        raise Errno::ENOENT, path
      end
    end

    def on_success_replace(aFilename)
      tmpfile = get_tempfile
      tmpFilename = tmpfile.path
      tmpfile.close
      if yield tmpFilename
        File.rename(tmpFilename, name)
      end
    end

    def get_tempfile
      tempFile = Tempfile.new(File.basename(name), File.dirname(name))
      tempFile.binmode
      tempFile
    end

  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
