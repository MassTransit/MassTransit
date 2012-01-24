module Zip
  class NullInputStream < NullDecompressor  #:nodoc:all
    include IOExtras::AbstractInputStream
  end
end

# Copyright (C) 2002, 2003 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
