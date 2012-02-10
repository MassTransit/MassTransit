module OpenStructToHash
  def to_hash
    # from: http://snippets.dzone.com/posts/show/7312
    h = @table
    # handles nested structures
    h.each do |k,v|
      if v.class == OpenStruct
        h[k] = v._to_hash
      end
    end
    return h
  end
end
