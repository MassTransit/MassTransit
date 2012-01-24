module Albacore
  def self.create_task(taskname, taskclass)
    # this style of creating tasks is not really what i
    # want to do. but it's necessary for ruby 1.8.6
    # because that version doesn't support the foo do |*args, &block|
    # block signature. it supports *args, but not &block.
    # so that limitation is worked around with string eval
    Object.class_eval(<<-EOF, __FILE__, __LINE__)
      def #{taskname}(name=:#{taskname}, *args, &configblock)
        task name, *args do |t, task_args|
          obj = #{taskclass}.new
          obj.load_config_by_task_name(name) if obj.respond_to?(:load_config_by_task_name)

          if !configblock.nil?
            case configblock.arity
              when 0
                configblock.call
              when 1
                configblock.call(obj)
              when 2
                configblock.call(obj, task_args)
            end
          end

          obj.execute if obj.respond_to?(:execute)
        end
      end

      def #{taskname}!(name=:#{taskname}, *args, &configblock)
        task name, *args do |t, task_args|
          obj = #{taskclass}.new
          obj.load_config_by_task_name(name) if obj.respond_to?(:load_config_by_task_name)

          if !configblock.nil?
            case configblock.arity
              when 0
                configblock.call
              when 1
                configblock.call(obj)
              when 2
                configblock.call(obj, task_args)
            end
          end

          obj.execute if obj.respond_to?(:execute)
        end.invoke
      end
    EOF
  end
end
