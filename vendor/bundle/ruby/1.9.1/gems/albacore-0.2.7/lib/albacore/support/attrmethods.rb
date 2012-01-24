module AttrMethods
  def attr_array(*names)
    names.each do |n|
      self.send :define_method, n do |*value|
        if value.nil? || value.empty?
          return instance_variable_get("@#{n}")
        else
          self.send "#{n}=".to_sym, value.to_ary.flatten
        end
      end
      self.send :define_method, "#{n}=" do |value|
        instance_variable_set("@#{n}", value)
      end
    end
  end

  def attr_hash(*names)
    names.each do |n|
      self.class_eval(<<-EOF, __FILE__, __LINE__)
        def #{n}(*value)
          if value.nil? || value.empty?
            instance_variable_get("@#{n}")
          else
            instance_variable_set("@#{n}", value[0])
          end
        end
      EOF
     self.send :define_method, "#{n}=" do |value|
        instance_variable_set("@#{n}", value)
      end
    end
  end
end
