module Zip
  class ZipExtraField < Hash
    ID_MAP = {}

    # Meta class for extra fields
    class Generic
      def self.register_map
        if self.const_defined?(:HEADER_ID)
          ID_MAP[self.const_get(:HEADER_ID)] = self
        end
      end

      def self.name
        self.to_s.split("::")[-1]
      end

      # return field [size, content] or false
      def initial_parse(binstr)
        if ! binstr
          # If nil, start with empty.
          return false
        elsif binstr[0,2] != self.class.const_get(:HEADER_ID)
          $stderr.puts "Warning: weired extra feild header ID. skip parsing"
          return false
        end
        [binstr[2,2].unpack("v")[0], binstr[4..-1]]
      end

      def ==(other)
        self.class != other.class and return false
        each { |k, v|
          v != other[k] and return false
        }
        true
      end

      def to_local_bin
        s = pack_for_local
        self.class.const_get(:HEADER_ID) + [s.length].pack("v") + s
      end

      def to_c_dir_bin
        s = pack_for_c_dir
        self.class.const_get(:HEADER_ID) + [s.length].pack("v") + s
      end
    end

    # Info-ZIP Additional timestamp field
    class UniversalTime < Generic
      HEADER_ID = "UT"
      register_map

      def initialize(binstr = nil)
        @ctime = nil
        @mtime = nil
        @atime = nil
        @flag  = nil
        binstr and merge(binstr)
      end
      attr_accessor :atime, :ctime, :mtime, :flag

      def merge(binstr)
        binstr == "" and return
        size, content = initial_parse(binstr)
        size or return
        @flag, mtime, atime, ctime = content.unpack("CVVV")
        mtime and @mtime ||= Time.at(mtime)
        atime and @atime ||= Time.at(atime)
        ctime and @ctime ||= Time.at(ctime)
      end

      def ==(other)
        @mtime == other.mtime &&
        @atime == other.atime &&
        @ctime == other.ctime
      end

      def pack_for_local
        s = [@flag].pack("C")
        @flag & 1 != 0 and s << [@mtime.to_i].pack("V")
        @flag & 2 != 0 and s << [@atime.to_i].pack("V")
        @flag & 4 != 0 and s << [@ctime.to_i].pack("V")
        s
      end

      def pack_for_c_dir
        s = [@flag].pack("C")
        @flag & 1 == 1 and s << [@mtime.to_i].pack("V")
        s
      end
    end

    # Info-ZIP Extra for UNIX uid/gid
    class IUnix < Generic
      HEADER_ID = "Ux"
      register_map

      def initialize(binstr = nil)
        @uid = 0
        @gid = 0
        binstr and merge(binstr)
      end
      attr_accessor :uid, :gid

      def merge(binstr)
        binstr == "" and return
        size, content = initial_parse(binstr)
        # size: 0 for central direcotry. 4 for local header
        return if(! size || size == 0)
        uid, gid = content.unpack("vv")
        @uid ||= uid
        @gid ||= gid
      end

      def ==(other)
        @uid == other.uid &&
        @gid == other.gid
      end

      def pack_for_local
        [@uid, @gid].pack("vv")
      end

      def pack_for_c_dir
        ""
      end
    end

    ## start main of ZipExtraField < Hash
    def initialize(binstr = nil)
      binstr and merge(binstr)
    end

    def merge(binstr)
      binstr == "" and return
      i = 0 
      while i < binstr.length
        id = binstr[i,2]
        len = binstr[i+2,2].to_s.unpack("v")[0] 
        if id && ID_MAP.member?(id)
          field_name = ID_MAP[id].name
          if self.member?(field_name)
            self[field_name].mergea(binstr[i, len+4])
          else
            field_obj = ID_MAP[id].new(binstr[i, len+4])
            self[field_name] = field_obj
          end
        elsif id
          unless self["Unknown"]
            s = ""
            class << s
              alias_method :to_c_dir_bin, :to_s
              alias_method :to_local_bin, :to_s
            end
            self["Unknown"] = s
          end
          if ! len || len+4 > binstr[i..-1].length
            self["Unknown"] << binstr[i..-1]
            break;
          end
          self["Unknown"] << binstr[i, len+4]
        end
        i += len+4
      end
    end

    def create(name)
      field_class = nil
      ID_MAP.each { |id, klass|
        if klass.name == name
          field_class = klass
          break
        end
      }
      if ! field_class
        raise ZipError, "Unknown extra field '#{name}'"
      end
      self[name] = field_class.new()
    end

    def to_local_bin
      s = ""
      each { |k, v|
        s << v.to_local_bin
      }
      s
    end
    alias :to_s :to_local_bin

    def to_c_dir_bin
      s = ""
      each { |k, v|
        s << v.to_c_dir_bin
      }
      s
    end

    def c_dir_length
      to_c_dir_bin.length
    end
    def local_length
      to_local_bin.length
    end
    alias :c_dir_size :c_dir_length
    alias :local_size :local_length
    alias :length     :local_length
    alias :size       :local_length
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
