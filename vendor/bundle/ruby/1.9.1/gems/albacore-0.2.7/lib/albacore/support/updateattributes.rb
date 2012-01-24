module UpdateAttributes
  def update_attributes(attrs)
    attrs.each do |key, value|
      setter = "#{key}="
      send(setter, value) if respond_to?(setter)
      @logger.warn "#{key} is not a settable attribute on #{self.class}" unless respond_to?(setter)
    end
  end

  def <<(attrs)
    update_attributes attrs
  end
end
