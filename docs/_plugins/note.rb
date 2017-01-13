module Jekyll
  class NoteBlock < Liquid::Block

    def initialize(tag_name, markup, options)
      @tag = markup
      super
    end

    def render(context)
      contents = super

      # pipe param through liquid to make additional replacements possible
      content = Liquid::Template.parse(contents).render context

      #strip out special chars and replace spaces with hyphens
      safeContent = content.rstrip.gsub(/[^\w\s]/,'').gsub(/[\s]/,'-')

      output = '<div class="w100 br3 hidden ba b--lightest-blue mv4">'
      output += '<div class="f4 black-60 bg-lightest-blue pv2 ph3 mv0 br3 br--top">Note</div>'
      output += '<div class="f5 lh-copy measure pa3">'
      output += content.strip
      output += "</div>"
      output += "</div>"

      output
    end
  end
end

Liquid::Template.register_tag("note", Jekyll::NoteBlock)
