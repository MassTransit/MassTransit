module Zip
  class ZipEntry
    STORED = 0
    DEFLATED = 8

    FSTYPE_FAT = 0
    FSTYPE_AMIGA = 1
    FSTYPE_VMS = 2
    FSTYPE_UNIX = 3
    FSTYPE_VM_CMS = 4
    FSTYPE_ATARI = 5
    FSTYPE_HPFS = 6
    FSTYPE_MAC = 7
    FSTYPE_Z_SYSTEM = 8 
    FSTYPE_CPM = 9
    FSTYPE_TOPS20 = 10
    FSTYPE_NTFS = 11
    FSTYPE_QDOS = 12
    FSTYPE_ACORN = 13
    FSTYPE_VFAT = 14
    FSTYPE_MVS = 15
    FSTYPE_BEOS = 16
    FSTYPE_TANDEM = 17
    FSTYPE_THEOS = 18
    FSTYPE_MAC_OSX = 19
    FSTYPE_ATHEOS = 30

    FSTYPES = {
      FSTYPE_FAT => 'FAT'.freeze,
      FSTYPE_AMIGA => 'Amiga'.freeze,
      FSTYPE_VMS => 'VMS (Vax or Alpha AXP)'.freeze,
      FSTYPE_UNIX => 'Unix'.freeze,
      FSTYPE_VM_CMS => 'VM/CMS'.freeze,
      FSTYPE_ATARI => 'Atari ST'.freeze,
      FSTYPE_HPFS => 'OS/2 or NT HPFS'.freeze,
      FSTYPE_MAC => 'Macintosh'.freeze,
      FSTYPE_Z_SYSTEM => 'Z-System'.freeze,
      FSTYPE_CPM => 'CP/M'.freeze,
      FSTYPE_TOPS20 => 'TOPS-20'.freeze,
      FSTYPE_NTFS => 'NTFS'.freeze,
      FSTYPE_QDOS => 'SMS/QDOS'.freeze,
      FSTYPE_ACORN => 'Acorn RISC OS'.freeze,
      FSTYPE_VFAT => 'Win32 VFAT'.freeze,
      FSTYPE_MVS => 'MVS'.freeze,
      FSTYPE_BEOS => 'BeOS'.freeze,
      FSTYPE_TANDEM => 'Tandem NSK'.freeze,
      FSTYPE_THEOS => 'Theos'.freeze,
      FSTYPE_MAC_OSX => 'Mac OS/X (Darwin)'.freeze,
      FSTYPE_ATHEOS => 'AtheOS'.freeze,
    }.freeze

    attr_accessor  :comment, :compressed_size, :crc, :extra, :compression_method, 
      :name, :size, :localHeaderOffset, :zipfile, :fstype, :externalFileAttributes, :gp_flags, :header_signature

    attr_accessor :follow_symlinks
    attr_accessor :restore_times, :restore_permissions, :restore_ownership
    attr_accessor :unix_uid, :unix_gid, :unix_perms

    attr_accessor :dirty

    attr_reader :ftype, :filepath # :nodoc:

    # Returns the character encoding used for name and comment
    def name_encoding
      (@gp_flags & 0b100000000000) != 0 ? "utf8" : "CP437//"
    end

    # Returns the name in the encoding specified by enc
    def name_in(enc)
      if RUBY_VERSION >= '1.9'
        @name.encode(enc)
      else
        Iconv.conv(enc, name_encoding, @name)
      end
    end

    # Returns the name in the encoding specified by enc
    def comment_in(enc)
      name_in(enc)
    end

    def initialize(zipfile = "", name = "", comment = "", extra = "", 
                   compressed_size = 0, crc = 0, 
       compression_method = ZipEntry::DEFLATED, size = 0,
       time  = Time.now)
      super()
      if name.start_with?("/")
        raise ZipEntryNameError, "Illegal ZipEntry name '#{name}', name must not start with /"
      end
      @localHeaderOffset = 0
      @local_header_size = 0
      @internalFileAttributes = 1
      @externalFileAttributes = 0
      @version = 52 # this library's version
      @ftype = nil # unspecified or unknown
      @filepath = nil
      if Zip::RUNNING_ON_WINDOWS
        @fstype = FSTYPE_FAT
      else
        @fstype = FSTYPE_UNIX
      end
      @zipfile = zipfile
      @comment = comment
      @compressed_size = compressed_size
      @crc = crc
      @extra = extra
      @compression_method = compression_method
      @name = name
      @size = size
      @time = time
      @gp_flags = nil

      @follow_symlinks = false

      @restore_times = true
      @restore_permissions = false
      @restore_ownership = false

# BUG: need an extra field to support uid/gid's
      @unix_uid = nil
      @unix_gid = nil
      @unix_perms = nil
#      @posix_acl = nil
#      @ntfs_acl = nil

      if name_is_directory?
        @ftype = :directory
      else
        @ftype = :file
      end

      unless ZipExtraField === @extra
        @extra = ZipExtraField.new(@extra.to_s)
      end

      @dirty = false
    end

    def time
      if @extra["UniversalTime"]
        @extra["UniversalTime"].mtime
      else
        # Atandard time field in central directory has local time
        # under archive creator. Then, we can't get timezone.
        @time
      end
    end
    alias :mtime :time

    def time=(aTime)
      unless @extra.member?("UniversalTime")
        @extra.create("UniversalTime")
      end
      @extra["UniversalTime"].mtime = aTime
      @time = aTime
    end

    # Returns +true+ if the entry is a directory.
    def directory?
      raise ZipInternalError, "current filetype is unknown: #{self.inspect}" unless @ftype
      @ftype == :directory
    end
    alias :is_directory :directory?

    # Returns +true+ if the entry is a file.
    def file?
      raise ZipInternalError, "current filetype is unknown: #{self.inspect}" unless @ftype
      @ftype == :file
    end

    # Returns +true+ if the entry is a symlink.
    def symlink?
      raise ZipInternalError, "current filetype is unknown: #{self.inspect}" unless @ftype
      @ftype == :symlink
    end

    def name_is_directory?  #:nodoc:all
      (%r{\/$} =~ @name) != nil
    end

    def local_entry_offset  #:nodoc:all
      localHeaderOffset + @local_header_size
    end

    def calculate_local_header_size  #:nodoc:all
      LOCAL_ENTRY_STATIC_HEADER_LENGTH + (@name ?  @name.bytesize : 0) + (@extra ? @extra.local_size : 0)
    end

    def cdir_header_size  #:nodoc:all
      CDIR_ENTRY_STATIC_HEADER_LENGTH  + (@name ?  @name.bytesize : 0) + 
      (@extra ? @extra.c_dir_size : 0) + (@comment ? @comment.bytesize : 0)
    end

    def next_header_offset  #:nodoc:all
      local_entry_offset + self.compressed_size
    end

    # Extracts entry to file destPath (defaults to @name).
    def extract(destPath = @name, &onExistsProc)
      onExistsProc ||= proc { false }

      if directory?
        create_directory(destPath, &onExistsProc)
      elsif file?
        write_file(destPath, &onExistsProc)
      elsif symlink?
        create_symlink(destPath, &onExistsProc)
      else
        raise RuntimeError, "unknown file type #{self.inspect}"
      end

      self
    end

    def to_s
      @name
    end

    protected

    def ZipEntry.read_zip_short(io) # :nodoc:
      io.read(2).unpack('v')[0]
    end

    def ZipEntry.read_zip_long(io) # :nodoc:
      io.read(4).unpack('V')[0]
    end
    public

    LOCAL_ENTRY_SIGNATURE = 0x04034b50
    LOCAL_ENTRY_STATIC_HEADER_LENGTH = 30
    LOCAL_ENTRY_TRAILING_DESCRIPTOR_LENGTH = 4+4+4
    VERSION_NEEDED_TO_EXTRACT = 10

    def read_local_entry(io)  #:nodoc:all
      @localHeaderOffset = io.tell
      staticSizedFieldsBuf = io.read(LOCAL_ENTRY_STATIC_HEADER_LENGTH)
      unless (staticSizedFieldsBuf.size==LOCAL_ENTRY_STATIC_HEADER_LENGTH)
        raise ZipError, "Premature end of file. Not enough data for zip entry local header"
      end

      @header_signature       ,
      @version          ,
      @fstype           ,
      @gp_flags          ,
      @compression_method,
      lastModTime       ,
      lastModDate       ,
      @crc              ,
      @compressed_size   ,
      @size             ,
      nameLength        ,
      extraLength       = staticSizedFieldsBuf.unpack('VCCvvvvVVVvv')

      unless (@header_signature == LOCAL_ENTRY_SIGNATURE)
        raise ZipError, "Zip local header magic not found at location '#{localHeaderOffset}'"
      end
      set_time(lastModDate, lastModTime)

      @name              = io.read(nameLength)
      extra              = io.read(extraLength)

      if (extra && extra.bytesize != extraLength)
        raise ZipError, "Truncated local zip entry header"
      else
        if ZipExtraField === @extra
          @extra.merge(extra)
        else
          @extra = ZipExtraField.new(extra)
        end
      end
      @local_header_size = calculate_local_header_size
    end

    def ZipEntry.read_local_entry(io)
      entry = new(io.path)
      entry.read_local_entry(io)
      return entry
    rescue ZipError
      return nil
    end

    def write_local_entry(io)   #:nodoc:all
      @localHeaderOffset = io.tell

      io << 
      [LOCAL_ENTRY_SIGNATURE    ,
      VERSION_NEEDED_TO_EXTRACT , # version needed to extract
      0                         , # @gp_flags                  ,
      @compression_method        ,
      @time.to_binary_dos_time     , # @lastModTime              ,
      @time.to_binary_dos_date     , # @lastModDate              ,
      @crc                      ,
      @compressed_size           ,
      @size                     ,
      @name ? @name.bytesize   : 0,
      @extra? @extra.local_length : 0 ].pack('VvvvvvVVVvv')
      io << @name
      io << (@extra ? @extra.to_local_bin : "")
    end

    CENTRAL_DIRECTORY_ENTRY_SIGNATURE = 0x02014b50
    CDIR_ENTRY_STATIC_HEADER_LENGTH = 46

    def read_c_dir_entry(io)  #:nodoc:all
      staticSizedFieldsBuf = io.read(CDIR_ENTRY_STATIC_HEADER_LENGTH)
      unless (staticSizedFieldsBuf.size == CDIR_ENTRY_STATIC_HEADER_LENGTH)
        raise ZipError, "Premature end of file. Not enough data for zip cdir entry header"
      end

      @header_signature          ,
      @version               , # version of encoding software
      @fstype                , # filesystem type
      @versionNeededToExtract,
      @gp_flags               ,
      @compression_method     ,
      lastModTime            ,
      lastModDate            ,
      @crc                   ,
      @compressed_size        ,
      @size                  ,
      nameLength             ,
      extraLength            ,
      commentLength          ,
      diskNumberStart        ,
      @internalFileAttributes,
      @externalFileAttributes,
      @localHeaderOffset     ,
      @name                  ,
      @extra                 ,
      @comment               = staticSizedFieldsBuf.unpack('VCCvvvvvVVVvvvvvVV')

      unless (@header_signature == CENTRAL_DIRECTORY_ENTRY_SIGNATURE)
        raise ZipError, "Zip local header magic not found at location '#{localHeaderOffset}'"
      end
      set_time(lastModDate, lastModTime)

      @name                  = io.read(nameLength)
      if ZipExtraField === @extra
        @extra.merge(io.read(extraLength))
      else
        @extra = ZipExtraField.new(io.read(extraLength))
      end
      @comment               = io.read(commentLength)
      unless (@comment && @comment.bytesize == commentLength)
        raise ZipError, "Truncated cdir zip entry header"
      end

      case @fstype
      when FSTYPE_UNIX
        @unix_perms = (@externalFileAttributes >> 16) & 07777

        case (@externalFileAttributes >> 28)
        when 04
          @ftype = :directory
        when 010
          @ftype = :file
        when 012
          @ftype = :symlink
        else
          @ftype = :unknown
        end
      else
        if name_is_directory?
          @ftype = :directory
        else
          @ftype = :file
        end
      end
      @local_header_size = calculate_local_header_size
    end

    def ZipEntry.read_c_dir_entry(io)  #:nodoc:all
      entry = new(io.path)
      entry.read_c_dir_entry(io)
      return entry
    rescue ZipError
      return nil
    end

    def file_stat(path)	# :nodoc:
      if @follow_symlinks
        return File::stat(path)
      else
        return File::lstat(path)
      end
    end

    def get_extra_attributes_from_path(path)	# :nodoc:
      unless Zip::RUNNING_ON_WINDOWS
        stat = file_stat(path)
        @unix_uid = stat.uid
        @unix_gid = stat.gid
        @unix_perms = stat.mode & 07777
      end
    end

    def set_extra_attributes_on_path(destPath)	# :nodoc:
      return unless (file? or directory?)

      case @fstype
      when FSTYPE_UNIX
        # BUG: does not update timestamps into account
        # ignore setuid/setgid bits by default.  honor if @restore_ownership
        unix_perms_mask = 01777
        unix_perms_mask = 07777 if (@restore_ownership)
        FileUtils::chmod(@unix_perms & unix_perms_mask, destPath) if (@restore_permissions && @unix_perms)
        FileUtils::chown(@unix_uid, @unix_gid, destPath) if (@restore_ownership && @unix_uid && @unix_gid && Process::egid == 0)
        # File::utimes()
      end
    end

    def write_c_dir_entry(io)  #:nodoc:all
      case @fstype
      when FSTYPE_UNIX
        ft = nil
        case @ftype
        when :file
          ft = 010
          @unix_perms ||= 0644
        when :directory
          ft = 004
          @unix_perms ||= 0755
        when :symlink
          ft = 012
          @unix_perms ||= 0755
        end

        if (!ft.nil?)
          @externalFileAttributes = (ft << 12 | (@unix_perms & 07777)) << 16
        end
      end

      io << 
      [CENTRAL_DIRECTORY_ENTRY_SIGNATURE,
      @version                          , # version of encoding software
      @fstype                           , # filesystem type
      VERSION_NEEDED_TO_EXTRACT         , # @versionNeededToExtract           ,
      0                                 , # @gp_flags                          ,
      @compression_method               ,
      @time.to_binary_dos_time          , # @lastModTime                      ,
      @time.to_binary_dos_date          , # @lastModDate                      ,
      @crc                              ,
      @compressed_size                  ,
      @size                             ,
      @name  ?  @name.bytesize  : 0       ,
      @extra ? @extra.c_dir_length : 0  ,
      @comment ? @comment.bytesize : 0    ,
      0                                 , # disk number start
      @internalFileAttributes           , # file type (binary=0, text=1)
      @externalFileAttributes           , # native filesystem attributes
      @localHeaderOffset                ,
      @name                             ,
      @extra                            ,
      @comment                          ].pack('VCCvvvvvVVVvvvvvVV')

      io << @name
      io << (@extra ? @extra.to_c_dir_bin : "")
      io << @comment
    end

    def == (other)
      return false unless other.class == self.class
      # Compares contents of local entry and exposed fields
      (@compression_method == other.compression_method &&
       @crc               == other.crc		     &&
       @compressed_size   == other.compressed_size   &&
       @size              == other.size	             &&
       @name              == other.name	             &&
       @extra             == other.extra             &&
       @filepath          == other.filepath          &&
       self.time.dos_equals(other.time))
    end

    def <=> (other)
      return to_s <=> other.to_s
    end

    # Returns an IO like object for the given ZipEntry.
    # Warning: may behave weird with symlinks.
    def get_input_stream(&aProc)
      if @ftype == :directory
          return yield(NullInputStream.instance) if block_given?
          return NullInputStream.instance
      elsif @filepath
        case @ftype
        when :file
          return File.open(@filepath, "rb", &aProc)

        when :symlink
          linkpath = File::readlink(@filepath)
          stringio = StringIO.new(linkpath)
          return yield(stringio) if block_given?
          return stringio
        else
          raise "unknown @ftype #{@ftype}"
        end
      else
        zis = ZipInputStream.new(@zipfile, localHeaderOffset)
        zis.get_next_entry
        if block_given?
          begin
            return yield(zis)
          ensure
            zis.close
          end
        else
          return zis
        end
      end
    end

    def gather_fileinfo_from_srcpath(srcPath) # :nodoc:
      stat = file_stat(srcPath)
      case stat.ftype
      when 'file'
        if name_is_directory?
          raise ArgumentError,
          "entry name '#{newEntry}' indicates directory entry, but "+
          "'#{srcPath}' is not a directory"
        end
        @ftype = :file
      when 'directory'
        if ! name_is_directory?
          @name += "/"
        end
        @ftype = :directory
      when 'link'
        if name_is_directory?
          raise ArgumentError,
          "entry name '#{newEntry}' indicates directory entry, but "+
          "'#{srcPath}' is not a directory"
        end
        @ftype = :symlink
      else
        raise RuntimeError, "unknown file type: #{srcPath.inspect} #{stat.inspect}"
      end

      @filepath = srcPath
      get_extra_attributes_from_path(@filepath)
    end

    def write_to_zip_output_stream(aZipOutputStream)  #:nodoc:all
      if @ftype == :directory
        aZipOutputStream.put_next_entry(self)
      elsif @filepath
        aZipOutputStream.put_next_entry(self)
        get_input_stream { |is| IOExtras.copy_stream(aZipOutputStream, is) } 
      else
        aZipOutputStream.copy_raw_entry(self)
      end
    end

    def parent_as_string
      entry_name = name.chomp("/")
      slash_index = entry_name.rindex("/")
      slash_index ? entry_name.slice(0, slash_index+1) : nil
    end

    def get_raw_input_stream(&aProc)
      File.open(@zipfile, "rb", &aProc)
    end

    private

    def set_time(binaryDosDate, binaryDosTime)
      @time = Time.parse_binary_dos_format(binaryDosDate, binaryDosTime)
    rescue ArgumentError
      puts "Invalid date/time in zip entry"
    end

    def write_file(destPath, continueOnExistsProc = proc { false })
      if File.exists?(destPath) && ! yield(self, destPath)
        raise ZipDestinationFileExistsError,
          "Destination '#{destPath}' already exists"
      end
      File.open(destPath, "wb") do |os|
        get_input_stream do |is|
          set_extra_attributes_on_path(destPath)

          buf = ''
          while buf = is.sysread(Decompressor::CHUNK_SIZE, buf)
            os << buf
          end
        end
      end
    end

    def create_directory(destPath)
      if File.directory? destPath
        return
      elsif File.exists? destPath
        if block_given? && yield(self, destPath)
          FileUtils::rm_f destPath
        else
          raise ZipDestinationFileExistsError,
            "Cannot create directory '#{destPath}'. "+
            "A file already exists with that name"
        end
      end
      Dir.mkdir destPath
      set_extra_attributes_on_path(destPath)
    end

# BUG: create_symlink() does not use &onExistsProc
    def create_symlink(destPath)
      stat = nil
      begin
        stat = File::lstat(destPath)
      rescue Errno::ENOENT
      end

      io = get_input_stream
      linkto = io.read

      if stat
        if stat.symlink?
          if File::readlink(destPath) == linkto
            return
          else
            raise ZipDestinationFileExistsError,
              "Cannot create symlink '#{destPath}'. "+
              "A symlink already exists with that name"
          end
        else
          raise ZipDestinationFileExistsError,
            "Cannot create symlink '#{destPath}'. "+
            "A file already exists with that name"
        end
      end

      File::symlink(linkto, destPath)
    end
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
