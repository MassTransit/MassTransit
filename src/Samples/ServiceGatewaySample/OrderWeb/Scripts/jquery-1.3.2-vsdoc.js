/*
 * This file has been commented to support Visual Studio Intellisense.
 * You should not use this file at runtime inside the browser--it is only
 * intended to be used only for design-time IntelliSense.  Please use the
 * standard jQuery library for all production use.
 *
 * Comment version: 1.3.2a
 */

/*
 * jQuery JavaScript Library v1.3.2
 *
 * Copyright (c) 2009 John Resig, http://jquery.com/
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 * Date: 2009-02-19 17:34:21 -0500 (Thu, 19 Feb 2009)
 * Revision: 6246
 */

(function(){

var 
	// Will speed up references to window, and allows munging its name.
	window = this,
	// Will speed up references to undefined, and allows munging its name.
	undefined,
	// Map over jQuery in case of overwrite
	_jQuery = window.jQuery,
	// Map over the $ in case of overwrite
	_$ = window.$,

	jQuery = window.jQuery = window.$ = function(selector, context) {
        ///	<summary>
        ///		1: $(expression, context) - This function accepts a string containing a CSS selector which is then used to match a set of elements.
        ///		2: $(html) - Create DOM elements on-the-fly from the provided String of raw HTML.
        ///		3: $(elements) - Wrap jQuery functionality around a single or multiple DOM Element(s).
        ///		4: $(callback) - A shorthand for $(document).ready().
        ///	</summary>
        ///	<param name="selector" type="String">
        ///		1: expression - An expression to search with.
        ///		2: html - A string of HTML to create on the fly.
        ///		3: elements - DOM element(s) to be encapsulated by a jQuery object.
        ///		4: callback - The function to execute when the DOM is ready.
        ///	</param>
        ///	<param name="context" type="jQuery">
        ///		1: context - A DOM Element, Document or jQuery to use as context.
        ///	</param>
        /// <field name="selector" Type="Object">
        ///     The DOM node context originally passed to jQuery() (if none was passed then context will be equal to the document).
        /// </field>
        /// <field name="context" Type="String">
        ///     A selector representing selector originally passed to jQuery().
        /// </field>
        ///	<returns type="jQuery" />
	
		// The jQuery object is actually just the init constructor 'enhanced'
		return new jQuery.fn.init( selector, context );
	},

	// A simple way to check for HTML strings or ID strings
	// (both of which we optimize for)
	quickExpr = /^[^<]*(<(.|\s)+>)[^>]*$|^#([\w-]+)$/,
	// Is it a simple selector
	isSimple = /^.[^:#\[\.,]*$/;

jQuery.fn = jQuery.prototype = {
	init: function( selector, context ) {
		///	<summary>
		///		1: $(expression, context) - This function accepts a string containing a CSS selector which is then used to match a set of elements.
		///		2: $(html) - Create DOM elements on-the-fly from the provided String of raw HTML.
		///		3: $(elements) - Wrap jQuery functionality around a single or multiple DOM Element(s).
		///		4: $(callback) - A shorthand for $(document).ready().
		///	</summary>
		///	<param name="selector" type="String">
		///		1: expression - An expression to search with.
		///		2: html - A string of HTML to create on the fly.
		///		3: elements - DOM element(s) to be encapsulated by a jQuery object.
		///		4: callback - The function to execute when the DOM is ready.
		///	</param>
		///	<param name="context" type="jQuery">
		///		1: context - A DOM Element, Document or jQuery to use as context.
		///	</param>
		///	<returns type="jQuery" />

		// Make sure that a selection was provided
		selector = selector || document;

		// Handle $(DOMElement)
		if ( selector.nodeType ) {
			this[0] = selector;
			this.length = 1;
			this.context = selector;
			return this;
		}
		// Handle HTML strings
		if (typeof selector === "string") {
		    // Are we dealing with HTML string or an ID?
		    var match = quickExpr.exec(selector);

		    // Verify a match, and that no context was specified for #id
		    if (match && (match[1] || !context)) {

		        // HANDLE: $(html) -> $(array)
		        if (match[1])
		            selector = jQuery.clean([match[1]], context);

		        // HANDLE: $("#id")
		        else {
		            var elem = document.getElementById(match[3]);

		            // Handle the case where IE and Opera return items
		            // by name instead of ID
		            if (elem && elem.id != match[3])
		                return jQuery().find(selector);

		            // Otherwise, we inject the element directly into the jQuery object
		            var ret = jQuery(elem || []);
		            ret.context = document;
		            ret.selector = selector;
		            return ret;
		        }

		        // HANDLE: $(expr, [context])
		        // (which is just equivalent to: $(content).find(expr)
		    } else
		        return jQuery(context).find(selector);

		    // HANDLE: $(function)
		    // Shortcut for document ready
		} else if ( jQuery.isFunction( selector ) )
			return jQuery( document ).ready( selector );

		// Make sure that old selector state is passed along
		if ( selector.selector && selector.context ) {
			this.selector = selector.selector;
			this.context = selector.context;
		}

		return this.setArray(jQuery.isArray( selector ) ?
			selector :
			jQuery.makeArray(selector));
	},

	// Start with an empty selector
	selector: "",

	// The current version of jQuery being used
	jquery: "1.3.2",

	// The number of elements contained in the matched element set
	size: function() {
		///	<summary>
		///		The number of elements currently matched.
		///		Part of Core
		///	</summary>
		///	<returns type="Number" />

		return this.length;
	},

	// Get the Nth element in the matched element set OR
	// Get the whole matched element set as a clean array
	get: function( num ) {
		///	<summary>
		///		Access a single matched element. num is used to access the
		///		Nth element matched.
		///		Part of Core
		///	</summary>
		///	<returns type="Element" />
		///	<param name="num" type="Number">
		///		Access the element in the Nth position.
		///	</param>

		return num == undefined ?

			// Return a 'clean' array
			Array.prototype.slice.call( this ) :

			// Return just the object
			this[ num ];
	},

	// Take an array of elements and push it onto the stack
	// (returning the new matched element set)
	pushStack: function( elems, name, selector ) {
		///	<summary>
		///		Set the jQuery object to an array of elements, while maintaining
		///		the stack.
		///		Part of Core
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="elems" type="Elements">
		///		An array of elements
		///	</param>
		
		// Build a new jQuery matched element set
		var ret = jQuery( elems );

		// Add the old object onto the stack (as a reference)
		ret.prevObject = this;

		ret.context = this.context;

		if ( name === "find" )
			ret.selector = this.selector + (this.selector ? " " : "") + selector;
		else if ( name )
			ret.selector = this.selector + "." + name + "(" + selector + ")";

		// Return the newly-formed element set
		return ret;
	},

	// Force the current matched set of elements to become
	// the specified array of elements (destroying the stack in the process)
	// You should use pushStack() in order to do this, but maintain the stack
	setArray: function( elems ) {
		///	<summary>
		///		Set the jQuery object to an array of elements. This operation is
		///		completely destructive - be sure to use .pushStack() if you wish to maintain
		///		the jQuery stack.
		///		Part of Core
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="elems" type="Elements">
		///		An array of elements
		///	</param>
		
		// Resetting the length to 0, then using the native Array push
		// is a super-fast way to populate an object with array-like properties
		this.length = 0;
		Array.prototype.push.apply( this, elems );

		return this;
	},

	// Execute a callback for every element in the matched set.
	// (You can seed the arguments with an array of args, but this is
	// only used internally.)
	each: function( callback, args ) {
		///	<summary>
		///		Execute a function within the context of every matched element.
		///		This means that every time the passed-in function is executed
		///		(which is once for every element matched) the 'this' keyword
		///		points to the specific element.
		///		Additionally, the function, when executed, is passed a single
		///		argument representing the position of the element in the matched
		///		set.
		///		Part of Core
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="callback" type="Function">
		///		A function to execute
		///	</param>

		return jQuery.each( this, callback, args );
	},

	// Determine the position of an element within
	// the matched set of elements
	index: function( elem ) {
		///	<summary>
		///		Searches every matched element for the object and returns
		///		the index of the element, if found, starting with zero. 
		///		Returns -1 if the object wasn't found.
		///		Part of Core
		///	</summary>
		///	<returns type="Number" />
		///	<param name="elem" type="Element">
		///		Object to search for
		///	</param>

		// Locate the position of the desired element
		return jQuery.inArray(
			// If it receives a jQuery object, the first element is used
			elem && elem.jquery ? elem[0] : elem
		, this );
	},

	attr: function( name, value, type ) {
		///	<summary>
		///		Set a single property to a computed value, on all matched elements.
		///		Instead of a value, a function is provided, that computes the value.
		///		Part of DOM/Attributes
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="name" type="String">
		///		The name of the property to set.
		///	</param>
		///	<param name="value" type="Function">
		///		A function returning the value to set.
		///	</param>

		var options = name;

		// Look for the case where we're accessing a style value
		if ( typeof name === "string" )
			if ( value === undefined )
				return this[0] && jQuery[ type || "attr" ]( this[0], name );

			else {
				options = {};
				options[ name ] = value;
			}

		// Check to see if we're setting style values
		return this.each(function(i){
			// Set all the styles
			for ( name in options )
				jQuery.attr(
					type ?
						this.style :
						this,
					name, jQuery.prop( this, options[ name ], type, i, name )
				);
		});
	},

	css: function( key, value ) {
		///	<summary>
		///		Set a single style property to a value, on all matched elements.
		///		If a number is provided, it is automatically converted into a pixel value.
		///		Part of CSS
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="key" type="String">
		///		The name of the property to set.
		///	</param>
		///	<param name="value" type="String">
		///		The value to set the property to.
		///	</param>

		// ignore negative width and height values
		if ( (key == 'width' || key == 'height') && parseFloat(value) < 0 )
			value = undefined;
		return this.attr( key, value, "curCSS" );
	},

	text: function( text ) {
		///	<summary>
		///		Set the text contents of all matched elements.
		///		Similar to html(), but escapes HTML (replace &quot;&lt;&quot; and &quot;&gt;&quot; with their
		///		HTML entities).
		///		Part of DOM/Attributes
		///	</summary>
		///	<returns type="String" />
		///	<param name="text" type="String">
		///		The text value to set the contents of the element to.
		///	</param>

		if ( typeof text !== "object" && text != null )
			return this.empty().append( (this[0] && this[0].ownerDocument || document).createTextNode( text ) );

		var ret = "";

		jQuery.each( text || this, function(){
			jQuery.each( this.childNodes, function(){
				if ( this.nodeType != 8 )
					ret += this.nodeType != 1 ?
						this.nodeValue :
						jQuery.fn.text( [ this ] );
			});
		});

		return ret;
	},

	wrapAll: function( html ) {
		///	<summary>
		///		Wrap all matched elements with a structure of other elements.
		///		This wrapping process is most useful for injecting additional
		///		stucture into a document, without ruining the original semantic
		///		qualities of a document.
		///		This works by going through the first element
		///		provided and finding the deepest ancestor element within its
		///		structure - it is that element that will en-wrap everything else.
		///		This does not work with elements that contain text. Any necessary text
		///		must be added after the wrapping is done.
		///		Part of DOM/Manipulation
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="html" type="Element">
		///		A DOM element that will be wrapped around the target.
		///	</param>

		if ( this[0] ) {
			// The elements to wrap the target around
			var wrap = jQuery( html, this[0].ownerDocument ).clone();

			if ( this[0].parentNode )
				wrap.insertBefore( this[0] );

			wrap.map(function(){
				var elem = this;

				while ( elem.firstChild )
					elem = elem.firstChild;

				return elem;
			}).append(this);
		}

		return this;
	},

	wrapInner: function( html ) {
		///	<summary>
		///		Wraps the inner child contents of each matched elemenht (including text nodes) with an HTML structure.
		///	</summary>
		///	<param name="html" type="String">
		///		A string of HTML or a DOM element that will be wrapped around the target contents.
		///	</param>
		///	<returns type="jQuery" />

		return this.each(function(){
			jQuery( this ).contents().wrapAll( html );
		});
	},

	wrap: function( html ) {
		///	<summary>
		///		Wrap all matched elements with a structure of other elements.
		///		This wrapping process is most useful for injecting additional
		///		stucture into a document, without ruining the original semantic
		///		qualities of a document.
		///		This works by going through the first element
		///		provided and finding the deepest ancestor element within its
		///		structure - it is that element that will en-wrap everything else.
		///		This does not work with elements that contain text. Any necessary text
		///		must be added after the wrapping is done.
		///		Part of DOM/Manipulation
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="html" type="Element">
		///		A DOM element that will be wrapped around the target.
		///	</param>
		
		return this.each(function(){
			jQuery( this ).wrapAll( html );
		});
	},

	append: function() {
		///	<summary>
		///		Append content to the inside of every matched element.
		///		This operation is similar to doing an appendChild to all the
		///		specified elements, adding them into the document.
		///		Part of DOM/Manipulation
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="content" type="Content">
		///		Content to append to the target
		///	</param>

		return this.domManip(arguments, true, function(elem){
			if (this.nodeType == 1)
				this.appendChild( elem );
		});
	},

	prepend: function() {
		///	<summary>
		///		Prepend content to the inside of every matched element.
		///		This operation is the best way to insert elements
		///		inside, at the beginning, of all matched elements.
		///		Part of DOM/Manipulation
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="" type="Content">
		///		Content to prepend to the target.
		///	</param>

		return this.domManip(arguments, true, function(elem){
			if (this.nodeType == 1)
				this.insertBefore( elem, this.firstChild );
		});
	},

	before: function() {
		///	<summary>
		///		Insert content before each of the matched elements.
		///		Part of DOM/Manipulation
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="" type="Content">
		///		Content to insert before each target.
		///	</param>

		return this.domManip(arguments, false, function(elem){
			this.parentNode.insertBefore( elem, this );
		});
	},

	after: function() {
		///	<summary>
		///		Insert content after each of the matched elements.
		///		Part of DOM/Manipulation
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="" type="Content">
		///		Content to insert after each target.
		///	</param>

		return this.domManip(arguments, false, function(elem){
			this.parentNode.insertBefore( elem, this.nextSibling );
		});
	},

	end: function() {
		///	<summary>
		///		End the most recent 'destructive' operation, reverting the list of matched elements
		///		back to its previous state. After an end operation, the list of matched elements will
		///		revert to the last state of matched elements.
		///		If there was no destructive operation before, an empty set is returned.
		///		Part of DOM/Traversing
		///	</summary>
		///	<returns type="jQuery" />

		return this.prevObject || jQuery( [] );
	},

	// For internal use only.
	// Behaves like an Array's method, not like a jQuery method.
	push: [].push,
	sort: [].sort,
	splice: [].splice,

	find: function( selector ) {
		///	<summary>
		///		Searches for all elements that match the specified expression.
		///		This method is a good way to find additional descendant
		///		elements with which to process.
		///		All searching is done using a jQuery expression. The expression can be
		///		written using CSS 1-3 Selector syntax, or basic XPath.
		///		Part of DOM/Traversing
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="selector" type="String">
		///		An expression to search with.
		///	</param>
		///	<returns type="jQuery" />

		if ( this.length === 1 ) {
			var ret = this.pushStack( [], "find", selector );
			ret.length = 0;
			jQuery.find( selector, this[0], ret );
			return ret;
		} else {
			return this.pushStack( jQuery.unique(jQuery.map(this, function(elem){
				return jQuery.find( selector, elem );
			})), "find", selector );
		}
	},

	clone: function( events ) {
		///	<summary>
		///		Clone matched DOM Elements and select the clones. 
		///		This is useful for moving copies of the elements to another
		///		location in the DOM.
		///		Part of DOM/Manipulation
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="deep" type="Boolean" optional="true">
		///		(Optional) Set to false if you don't want to clone all descendant nodes, in addition to the element itself.
		///	</param>

		// Do the clone
		var ret = this.map(function(){
			if ( !jQuery.support.noCloneEvent && !jQuery.isXMLDoc(this) ) {
				// IE copies events bound via attachEvent when
				// using cloneNode. Calling detachEvent on the
				// clone will also remove the events from the orignal
				// In order to get around this, we use innerHTML.
				// Unfortunately, this means some modifications to
				// attributes in IE that are actually only stored
				// as properties will not be copied (such as the
				// the name attribute on an input).
				var html = this.outerHTML;
				if ( !html ) {
					var div = this.ownerDocument.createElement("div");
					div.appendChild( this.cloneNode(true) );
					html = div.innerHTML;
				}

				return jQuery.clean([html.replace(/ jQuery\d+="(?:\d+|null)"/g, "").replace(/^\s*/, "")])[0];
			} else
				return this.cloneNode(true);
		});

		// Copy the events from the original to the clone
		if ( events === true ) {
			var orig = this.find("*").andSelf(), i = 0;

			ret.find("*").andSelf().each(function(){
				if ( this.nodeName !== orig[i].nodeName )
					return;

				var events = jQuery.data( orig[i], "events" );

				for ( var type in events ) {
					for ( var handler in events[ type ] ) {
						jQuery.event.add( this, type, events[ type ][ handler ], events[ type ][ handler ].data );
					}
				}

				i++;
			});
		}

		// Return the cloned set
		return ret;
	},

	filter: function( selector ) {
		///	<summary>
		///		Removes all elements from the set of matched elements that do not
		///		pass the specified filter. This method is used to narrow down
		///		the results of a search.
		///		})
		///		Part of DOM/Traversing
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="selector" type="Function">
		///		A function to use for filtering
		///	</param>
		///	<returns type="jQuery" />
	
		return this.pushStack(
			jQuery.isFunction( selector ) &&
			jQuery.grep(this, function(elem, i){
				return selector.call( elem, i );
			}) ||

			jQuery.multiFilter( selector, jQuery.grep(this, function(elem){
				return elem.nodeType === 1;
			}) ), "filter", selector );
	},

	closest: function( selector ) {
		///	<summary>
		///		Get a set of elements containing the closest parent element that matches the specified selector, the starting element included.
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="selector" type="Function">
		///		An expression to filter the elements with.
		///	</param>
		///	<returns type="jQuery" />

		var pos = jQuery.expr.match.POS.test( selector ) ? jQuery(selector) : null,
			closer = 0;

		return this.map(function(){
			var cur = this;
			while ( cur && cur.ownerDocument ) {
				if ( pos ? pos.index(cur) > -1 : jQuery(cur).is(selector) ) {
					jQuery.data(cur, "closest", closer);
					return cur;
				}
				cur = cur.parentNode;
				closer++;
			}
		});
	},

	not: function( selector ) {
		///	<summary>
		///		Removes any elements inside the array of elements from the set
		///		of matched elements. This method is used to remove one or more
		///		elements from a jQuery object.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="selector" type="jQuery">
		///		A set of elements to remove from the jQuery set of matched elements.
		///	</param>
		///	<returns type="jQuery" />

		if ( typeof selector === "string" )
			// test special case where just one selector is passed in
			if ( isSimple.test( selector ) )
				return this.pushStack( jQuery.multiFilter( selector, this, true ), "not", selector );
			else
				selector = jQuery.multiFilter( selector, this );

		var isArrayLike = selector.length && selector[selector.length - 1] !== undefined && !selector.nodeType;
		return this.filter(function() {
			return isArrayLike ? jQuery.inArray( this, selector ) < 0 : this != selector;
		});
	},

	add: function( selector ) {
		///	<summary>
		///		Adds one or more Elements to the set of matched elements.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="elements" type="Element">
		///		One or more Elements to add
		///	</param>
		///	<returns type="jQuery" />
	
		return this.pushStack( jQuery.unique( jQuery.merge(
			this.get(),
			typeof selector === "string" ?
				jQuery( selector ) :
				jQuery.makeArray( selector )
		)));
	},

	is: function( selector ) {
		///	<summary>
		///		Checks the current selection against an expression and returns true,
		///		if at least one element of the selection fits the given expression.
		///		Does return false, if no element fits or the expression is not valid.
		///		filter(String) is used internally, therefore all rules that apply there
		///		apply here, too.
		///		Part of DOM/Traversing
		///	</summary>
		///	<returns type="Boolean" />
		///	<param name="expr" type="String">
		///		 The expression with which to filter
		///	</param>

		return !!selector && jQuery.multiFilter( selector, this ).length > 0;
	},

	hasClass: function( selector ) {
		///	<summary>
		///		Checks the current selection against a class and returns whether at least one selection has a given class.
		///	</summary>
		///	<param name="selector" type="String">The class to check against</param>
		///	<returns type="Boolean">True if at least one element in the selection has the class, otherwise false.</returns>

		return !!selector && this.is( "." + selector );
	},

	val: function( value ) {
		///	<summary>
		///		Set the value of every matched element.
		///		Part of DOM/Attributes
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="val" type="String">
		///		 Set the property to the specified value.
		///	</param>

		if ( value === undefined ) {			
			var elem = this[0];

			if ( elem ) {
				if( jQuery.nodeName( elem, 'option' ) )
					return (elem.attributes.value || {}).specified ? elem.value : elem.text;
				
				// We need to handle select boxes special
				if ( jQuery.nodeName( elem, "select" ) ) {
					var index = elem.selectedIndex,
						values = [],
						options = elem.options,
						one = elem.type == "select-one";

					// Nothing was selected
					if ( index < 0 )
						return null;

					// Loop through all the selected options
					for ( var i = one ? index : 0, max = one ? index + 1 : options.length; i < max; i++ ) {
						var option = options[ i ];

						if ( option.selected ) {
							// Get the specifc value for the option
							value = jQuery(option).val();

							// We don't need an array for one selects
							if ( one )
								return value;

							// Multi-Selects return an array
							values.push( value );
						}
					}

					return values;				
				}

				// Everything else, we just grab the value
				return (elem.value || "").replace(/\r/g, "");

			}

			return undefined;
		}

		if ( typeof value === "number" )
			value += '';

		return this.each(function(){
			if ( this.nodeType != 1 )
				return;

			if ( jQuery.isArray(value) && /radio|checkbox/.test( this.type ) )
				this.checked = (jQuery.inArray(this.value, value) >= 0 ||
					jQuery.inArray(this.name, value) >= 0);

			else if ( jQuery.nodeName( this, "select" ) ) {
				var values = jQuery.makeArray(value);

				jQuery( "option", this ).each(function(){
					this.selected = (jQuery.inArray( this.value, values ) >= 0 ||
						jQuery.inArray( this.text, values ) >= 0);
				});

				if ( !values.length )
					this.selectedIndex = -1;

			} else
				this.value = value;
		});
	},

	html: function( value ) {
		///	<summary>
		///		Set the html contents of every matched element.
		///		This property is not available on XML documents.
		///		Part of DOM/Attributes
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="val" type="String">
		///		 Set the html contents to the specified value.
		///	</param>

		return value === undefined ?
			(this[0] ?
				this[0].innerHTML.replace(/ jQuery\d+="(?:\d+|null)"/g, "") :
				null) :
			this.empty().append( value );
	},

	replaceWith: function( value ) {
		///	<summary>
		///		Replaces all matched element with the specified HTML or DOM elements.
		///	</summary>
		///	<param name="value" type="String">
		///		The content with which to replace the matched elements.
		///	</param>
		///	<returns type="jQuery">The element that was just replaced.</returns>

		return this.after( value ).remove();
	},

	eq: function( i ) {
		///	<summary>
		///		Reduce the set of matched elements to a single element.
		///		The position of the element in the set of matched elements
		///		starts at 0 and goes to length - 1.
		///		Part of Core
		///	</summary>
		///	<returns type="jQuery" />
		///	<param name="num" type="Number">
		///		pos The index of the element that you wish to limit to.
		///	</param>

		return this.slice( i, +i + 1 );
	},

	slice: function() {
		///	<summary>
		///		Selects a subset of the matched elements.  Behaves exactly like the built-in Array slice method.
		///	</summary>
		///	<param name="start" type="Number" integer="true">Where to start the subset (0-based).</param>
		///	<param name="end" optional="true" type="Number" integer="true">Where to end the subset (not including the end element itself).
		///		If omitted, ends at the end of the selection</param>
		///	<returns type="jQuery">The sliced elements</returns>

		return this.pushStack( Array.prototype.slice.apply( this, arguments ),
			"slice", Array.prototype.slice.call(arguments).join(",") );
	},

	map: function( callback ) {
		///	<summary>
		///		This member is internal.
		///	</summary>
		///	<private />
		///	<returns type="jQuery" />
		
		return this.pushStack( jQuery.map(this, function(elem, i){
			return callback.call( elem, i, elem );
		}));
	},

	andSelf: function() {
		///	<summary>
		///		Adds the previous selection to the current selection.
		///	</summary>
		///	<returns type="jQuery" />
		
		return this.add( this.prevObject );
	},

	domManip: function( args, table, callback ) {
		///	<param name="args" type="Array">
		///		 Args
		///	</param>
		///	<param name="table" type="Boolean">
		///		 Insert TBODY in TABLEs if one is not found.
		///	</param>
		///	<param name="dir" type="Number">
		///		 If dir&lt;0, process args in reverse order.
		///	</param>
		///	<param name="fn" type="Function">
		///		 The function doing the DOM manipulation.
		///	</param>
		///	<returns type="jQuery" />
		///	<summary>
		///		Part of Core
		///	</summary>
		
		if ( this[0] ) {
			var fragment = (this[0].ownerDocument || this[0]).createDocumentFragment(),
				scripts = jQuery.clean( args, (this[0].ownerDocument || this[0]), fragment ),
				first = fragment.firstChild;

			if ( first )
				for ( var i = 0, l = this.length; i < l; i++ )
					callback.call( root(this[i], first), this.length > 1 || i > 0 ?
							fragment.cloneNode(true) : fragment );
		
			if ( scripts )
				jQuery.each( scripts, evalScript );
		}

		return this;
		
		function root( elem, cur ) {
			return table && jQuery.nodeName(elem, "table") && jQuery.nodeName(cur, "tr") ?
				(elem.getElementsByTagName("tbody")[0] ||
				elem.appendChild(elem.ownerDocument.createElement("tbody"))) :
				elem;
		}
	}
};

// Give the init function the jQuery prototype for later instantiation
jQuery.fn.init.prototype = jQuery.fn;

function evalScript( i, elem ) {
	///	<summary>
	///		This method is internal.
	///	</summary>
	/// <private />
	
	if ( elem.src )
		jQuery.ajax({
			url: elem.src,
			async: false,
			dataType: "script"
		});

	else
		jQuery.globalEval( elem.text || elem.textContent || elem.innerHTML || "" );

	if ( elem.parentNode )
		elem.parentNode.removeChild( elem );
}

function now(){
	///	<summary>
	///		Gets the current date.
	///	</summary>
	///	<returns type="Date">The current date.</returns>
	return +new Date;
}

jQuery.extend = jQuery.fn.extend = function() {
	///	<summary>
	///		Extend one object with one or more others, returning the original,
	///		modified, object. This is a great utility for simple inheritance.
	///		jQuery.extend(settings, options);
	///		var settings = jQuery.extend({}, defaults, options);
	///		Part of JavaScript
	///	</summary>
	///	<param name="target" type="Object">
	///		 The object to extend
	///	</param>
	///	<param name="prop1" type="Object">
	///		 The object that will be merged into the first.
	///	</param>
	///	<param name="propN" type="Object" optional="true" parameterArray="true">
	///		 (optional) More objects to merge into the first
	///	</param>
	///	<returns type="Object" />

	// copy reference to target object
	var target = arguments[0] || {}, i = 1, length = arguments.length, deep = false, options;

	// Handle a deep copy situation
	if ( typeof target === "boolean" ) {
		deep = target;
		target = arguments[1] || {};
		// skip the boolean and the target
		i = 2;
	}

	// Handle case when target is a string or something (possible in deep copy)
	if ( typeof target !== "object" && !jQuery.isFunction(target) )
		target = {};

	// extend jQuery itself if only one argument is passed
	if ( length == i ) {
		target = this;
		--i;
	}

	for ( ; i < length; i++ )
		// Only deal with non-null/undefined values
		if ( (options = arguments[ i ]) != null )
			// Extend the base object
			for ( var name in options ) {
				var src = target[ name ], copy = options[ name ];

				// Prevent never-ending loop
				if ( target === copy )
					continue;

				// Recurse if we're merging object values
				if ( deep && copy && typeof copy === "object" && !copy.nodeType )
					target[ name ] = jQuery.extend( deep, 
						// Never move original objects, clone them
						src || ( copy.length != null ? [ ] : { } )
					, copy );

				// Don't bring in undefined values
				else if ( copy !== undefined )
					target[ name ] = copy;

			}

	// Return the modified object
	return target;
};

// exclude the following css properties to add px
var	exclude = /z-?index|font-?weight|opacity|zoom|line-?height/i,
	// cache defaultView
	defaultView = document.defaultView || {},
	toString = Object.prototype.toString;

jQuery.extend({
	noConflict: function( deep ) {
		///	<summary>
		///		Run this function to give control of the $ variable back
		///		to whichever library first implemented it. This helps to make 
		///		sure that jQuery doesn't conflict with the $ object
		///		of other libraries.
		///		By using this function, you will only be able to access jQuery
		///		using the 'jQuery' variable. For example, where you used to do
		///		$(&quot;div p&quot;), you now must do jQuery(&quot;div p&quot;).
		///		Part of Core 
		///	</summary>
		///	<returns type="undefined" />
		
		window.$ = _$;

		if ( deep )
			window.jQuery = _jQuery;

		return jQuery;
	},

	// See test/unit/core.js for details concerning isFunction.
	// Since version 1.3, DOM methods and functions like alert
	// aren't supported. They return false on IE (#2968).
	isFunction: function( obj ) {
		///	<summary>
		///		Determines if the parameter passed is a function.
		///	</summary>
		///	<param name="obj" type="Object">The object to check</param>
		///	<returns type="Boolean">True if the parameter is a function; otherwise false.</returns>
		
		return toString.call(obj) === "[object Function]";
	},

	isArray: function(obj) {
	    ///	<summary>
	    ///		Determine if the parameter passed is an array.
	    ///	</summary>
	    ///	<param name="obj" type="Object">Object to test whether or not it is an array.</param>
	    ///	<returns type="Boolean">True if the parameter is a function; otherwise false.</returns>
    		
		return toString.call(obj) === "[object Array]";
	},

	// check if an element is in a (or is an) XML document
	isXMLDoc: function( elem ) {
		///	<summary>
		///		Determines if the parameter passed is an XML document.
		///	</summary>
		///	<param name="elem" type="Object">The object to test</param>
		///	<returns type="Boolean">True if the parameter is an XML document; otherwise false.</returns>

	    return elem.nodeType === 9 && elem.documentElement.nodeName !== "HTML" ||
			!!elem.ownerDocument && jQuery.isXMLDoc(elem.ownerDocument);
    },

	// Evalulates a script in a global context
	globalEval: function( data ) {
		///	<summary>
		///		Internally evaluates a script in a global context.
		///	</summary>
		///	<private />

		if ( data && /\S/.test(data) ) {
			// Inspired by code by Andrea Giammarchi
			// http://webreflection.blogspot.com/2007/08/global-scope-evaluation-and-dom.html
			var head = document.getElementsByTagName("head")[0] || document.documentElement,
				script = document.createElement("script");

			script.type = "text/javascript";
			if ( jQuery.support.scriptEval )
				script.appendChild( document.createTextNode( data ) );
			else
				script.text = data;

			// Use insertBefore instead of appendChild  to circumvent an IE6 bug.
			// This arises when a base node is used (#2709).
			head.insertBefore( script, head.firstChild );
			head.removeChild( script );
		}
	},

	nodeName: function( elem, name ) {
		///	<summary>
		///		Checks whether the specified element has the specified DOM node name.
		///	</summary>
		///	<param name="elem" type="Element">The element to examine</param>
		///	<param name="name" type="String">The node name to check</param>
		///	<returns type="Boolean">True if the specified node name matches the node's DOM node name; otherwise false</returns>

		return elem.nodeName && elem.nodeName.toUpperCase() == name.toUpperCase();
	},

	// args is for internal usage only
	each: function( object, callback, args ) {
		///	<summary>
		///		A generic iterator function, which can be used to seemlessly
		///		iterate over both objects and arrays. This function is not the same
		///		as $().each() - which is used to iterate, exclusively, over a jQuery
		///		object. This function can be used to iterate over anything.
		///		The callback has two arguments:the key (objects) or index (arrays) as first
		///		the first, and the value as the second.
		///		Part of JavaScript
		///	</summary>
		///	<param name="obj" type="Object">
		///		 The object, or array, to iterate over.
		///	</param>
		///	<param name="fn" type="Function">
		///		 The function that will be executed on every object.
		///	</param>
		///	<returns type="Object" />
		
		var name, i = 0, length = object.length;

		if ( args ) {
			if ( length === undefined ) {
				for ( name in object )
					if ( callback.apply( object[ name ], args ) === false )
						break;
			} else
				for ( ; i < length; )
					if ( callback.apply( object[ i++ ], args ) === false )
						break;

		// A special, fast, case for the most common use of each
		} else {
			if ( length === undefined ) {
				for ( name in object )
					if ( callback.call( object[ name ], name, object[ name ] ) === false )
						break;
			} else
				for ( var value = object[0];
					i < length && callback.call( value, i, value ) !== false; value = object[++i] ){}
		}

		return object;
	},

	prop: function( elem, value, type, i, name ) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		// This member is not documented within the jQuery API: http://docs.jquery.com/action/edit/Internals/jQuery.prop

		// Handle executable functions
		if ( jQuery.isFunction( value ) )
			value = value.call( elem, i );

		// Handle passing in a number to a CSS property
		return typeof value === "number" && type == "curCSS" && !exclude.test( name ) ?
			value + "px" :
			value;
	},

	className: {
		// internal only, use addClass("class")
		add: function( elem, classNames ) {
   			///	<summary>
   			///		Internal use only; use addClass('class')
			///	</summary>
   			///	<private />

			jQuery.each((classNames || "").split(/\s+/), function(i, className){
				if ( elem.nodeType == 1 && !jQuery.className.has( elem.className, className ) )
					elem.className += (elem.className ? " " : "") + className;
			});
		},

		// internal only, use removeClass("class")
		remove: function( elem, classNames ) {
   			///	<summary>
   			///		Internal use only; use removeClass('class')
			///	</summary>
   			///	<private />

			if (elem.nodeType == 1)
				elem.className = classNames !== undefined ?
					jQuery.grep(elem.className.split(/\s+/), function(className){
						return !jQuery.className.has( classNames, className );
					}).join(" ") :
					"";
		},

		// internal only, use hasClass("class")
		has: function( elem, className ) {
   			///	<summary>
   			///		Internal use only; use hasClass('class')
			///	</summary>
   			///	<private />

		    return elem && jQuery.inArray(className, (elem.className || elem).toString().split(/\s+/)) > -1;
		}
	},

	// A method for quickly swapping in/out CSS properties to get correct calculations
	swap: function( elem, options, callback ) {
		///	<summary>
		///		Swap in/out style options.
		///	</summary>

		var old = {};
		// Remember the old values, and insert the new ones
		for ( var name in options ) {
			old[ name ] = elem.style[ name ];
			elem.style[ name ] = options[ name ];
		}

		callback.call( elem );

		// Revert the old values
		for ( var name in options )
			elem.style[ name ] = old[ name ];
	},

	css: function( elem, name, force, extra ) {
		///	<summary>
		///		This method is internal only.
		///	</summary>
		///	<private />
		// This method is undocumented in jQuery API: http://docs.jquery.com/action/edit/Internals/jQuery.css

		if ( name == "width" || name == "height" ) {
			var val, props = { position: "absolute", visibility: "hidden", display:"block" }, which = name == "width" ? [ "Left", "Right" ] : [ "Top", "Bottom" ];

			function getWH() {
				val = name == "width" ? elem.offsetWidth : elem.offsetHeight;

				if ( extra === "border" )
					return;

				jQuery.each( which, function() {
					if ( !extra )
						val -= parseFloat(jQuery.curCSS( elem, "padding" + this, true)) || 0;
					if ( extra === "margin" )
						val += parseFloat(jQuery.curCSS( elem, "margin" + this, true)) || 0;
					else
						val -= parseFloat(jQuery.curCSS( elem, "border" + this + "Width", true)) || 0;
				});
			}

			if ( elem.offsetWidth !== 0 )
				getWH();
			else
				jQuery.swap( elem, props, getWH );

			return Math.max(0, Math.round(val));
		}

		return jQuery.curCSS( elem, name, force );
	},

	curCSS: function( elem, name, force ) {
		///	<summary>
		///		This method is internal only.
		///	</summary>
		///	<private />
		// This method is undocumented in jQuery API: http://docs.jquery.com/action/edit/Internals/jQuery.curCSS

		var ret, style = elem.style;

		// We need to handle opacity special in IE
		if ( name == "opacity" && !jQuery.support.opacity ) {
			ret = jQuery.attr( style, "opacity" );

			return ret == "" ?
				"1" :
				ret;
		}

		// Make sure we're using the right name for getting the float value
		if ( name.match( /float/i ) )
			name = styleFloat;

		if ( !force && style && style[ name ] )
			ret = style[ name ];

		else if ( defaultView.getComputedStyle ) {

			// Only "float" is needed here
			if ( name.match( /float/i ) )
				name = "float";

			name = name.replace( /([A-Z])/g, "-$1" ).toLowerCase();

			var computedStyle = defaultView.getComputedStyle( elem, null );

			if ( computedStyle )
				ret = computedStyle.getPropertyValue( name );

			// We should always get a number back from opacity
			if ( name == "opacity" && ret == "" )
				ret = "1";

		} else if ( elem.currentStyle ) {
			var camelCase = name.replace(/\-(\w)/g, function(all, letter){
				return letter.toUpperCase();
			});

			ret = elem.currentStyle[ name ] || elem.currentStyle[ camelCase ];

			// From the awesome hack by Dean Edwards
			// http://erik.eae.net/archives/2007/07/27/18.54.15/#comment-102291

			// If we're not dealing with a regular pixel number
			// but a number that has a weird ending, we need to convert it to pixels
			if ( !/^\d+(px)?$/i.test( ret ) && /^\d/.test( ret ) ) {
				// Remember the original values
				var left = style.left, rsLeft = elem.runtimeStyle.left;

				// Put in the new values to get a computed value out
				elem.runtimeStyle.left = elem.currentStyle.left;
				style.left = ret || 0;
				ret = style.pixelLeft + "px";

				// Revert the changed values
				style.left = left;
				elem.runtimeStyle.left = rsLeft;
			}
		}

		return ret;
	},

	clean: function( elems, context, fragment ) {
		///	<summary>
		///		This method is internal only.
		///	</summary>
		///	<private />
		// This method is undocumented in the jQuery API: http://docs.jquery.com/action/edit/Internals/jQuery.clean


		context = context || document;

		// !context.createElement fails in IE with an error but returns typeof 'object'
		if ( typeof context.createElement === "undefined" )
			context = context.ownerDocument || context[0] && context[0].ownerDocument || document;

		// If a single string is passed in and it's a single tag
		// just do a createElement and skip the rest
		if ( !fragment && elems.length === 1 && typeof elems[0] === "string" ) {
			var match = /^<(\w+)\s*\/?>$/.exec(elems[0]);
			if ( match )
				return [ context.createElement( match[1] ) ];
		}

		var ret = [], scripts = [], div = context.createElement("div");

		jQuery.each(elems, function(i, elem){
			if ( typeof elem === "number" )
				elem += '';

			if ( !elem )
				return;

			// Convert html string into DOM nodes
			if ( typeof elem === "string" ) {
				// Fix "XHTML"-style tags in all browsers
				elem = elem.replace(/(<(\w+)[^>]*?)\/>/g, function(all, front, tag){
					return tag.match(/^(abbr|br|col|img|input|link|meta|param|hr|area|embed)$/i) ?
						all :
						front + "></" + tag + ">";
				});

				// Trim whitespace, otherwise indexOf won't work as expected
				var tags = elem.replace(/^\s+/, "").substring(0, 10).toLowerCase();

				var wrap =
					// option or optgroup
					!tags.indexOf("<opt") &&
					[ 1, "<select multiple='multiple'>", "</select>" ] ||

					!tags.indexOf("<leg") &&
					[ 1, "<fieldset>", "</fieldset>" ] ||

					tags.match(/^<(thead|tbody|tfoot|colg|cap)/) &&
					[ 1, "<table>", "</table>" ] ||

					!tags.indexOf("<tr") &&
					[ 2, "<table><tbody>", "</tbody></table>" ] ||

				 	// <thead> matched above
					(!tags.indexOf("<td") || !tags.indexOf("<th")) &&
					[ 3, "<table><tbody><tr>", "</tr></tbody></table>" ] ||

					!tags.indexOf("<col") &&
					[ 2, "<table><tbody></tbody><colgroup>", "</colgroup></table>" ] ||

					// IE can't serialize <link> and <script> tags normally
					!jQuery.support.htmlSerialize &&
					[ 1, "div<div>", "</div>" ] ||

					[ 0, "", "" ];

				// Go to html and back, then peel off extra wrappers
				div.innerHTML = wrap[1] + elem + wrap[2];

				// Move to the right depth
				while ( wrap[0]-- )
					div = div.lastChild;

				// Remove IE's autoinserted <tbody> from table fragments
				if ( !jQuery.support.tbody ) {

					// String was a <table>, *may* have spurious <tbody>
					var hasBody = /<tbody/i.test(elem),
						tbody = !tags.indexOf("<table") && !hasBody ?
							div.firstChild && div.firstChild.childNodes :

						// String was a bare <thead> or <tfoot>
						wrap[1] == "<table>" && !hasBody ?
							div.childNodes :
							[];

					for ( var j = tbody.length - 1; j >= 0 ; --j )
						if ( jQuery.nodeName( tbody[ j ], "tbody" ) && !tbody[ j ].childNodes.length )
							tbody[ j ].parentNode.removeChild( tbody[ j ] );

					}

				// IE completely kills leading whitespace when innerHTML is used
				if ( !jQuery.support.leadingWhitespace && /^\s/.test( elem ) )
					div.insertBefore( context.createTextNode( elem.match(/^\s*/)[0] ), div.firstChild );
				
				elem = jQuery.makeArray( div.childNodes );
			}

			if ( elem.nodeType )
				ret.push( elem );
			else
				ret = jQuery.merge( ret, elem );

		});

		if ( fragment ) {
			for ( var i = 0; ret[i]; i++ ) {
				if ( jQuery.nodeName( ret[i], "script" ) && (!ret[i].type || ret[i].type.toLowerCase() === "text/javascript") ) {
					scripts.push( ret[i].parentNode ? ret[i].parentNode.removeChild( ret[i] ) : ret[i] );
				} else {
					if ( ret[i].nodeType === 1 )
						ret.splice.apply( ret, [i + 1, 0].concat(jQuery.makeArray(ret[i].getElementsByTagName("script"))) );
					fragment.appendChild( ret[i] );
				}
			}
			
			return scripts;
		}

		return ret;
	},

	attr: function( elem, name, value ) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />

		// don't set attributes on text and comment nodes
		if (!elem || elem.nodeType == 3 || elem.nodeType == 8)
			return undefined;

		var notxml = !jQuery.isXMLDoc( elem ),
			// Whether we are setting (or getting)
			set = value !== undefined;

		// Try to normalize/fix the name
		name = notxml && jQuery.props[ name ] || name;

		// Only do all the following if this is a node (faster for style)
		// IE elem.getAttribute passes even for style
		if ( elem.tagName ) {

			// These attributes require special treatment
			var special = /href|src|style/.test( name );

			// Safari mis-reports the default selected property of a hidden option
			// Accessing the parent's selectedIndex property fixes it
			if ( name == "selected" && elem.parentNode )
				elem.parentNode.selectedIndex;

			// If applicable, access the attribute via the DOM 0 way
			if ( name in elem && notxml && !special ) {
				if ( set ){
					// We can't allow the type property to be changed (since it causes problems in IE)
					if ( name == "type" && jQuery.nodeName( elem, "input" ) && elem.parentNode )
						throw "type property can't be changed";

					elem[ name ] = value;
				}

				// browsers index elements by id/name on forms, give priority to attributes.
				if( jQuery.nodeName( elem, "form" ) && elem.getAttributeNode(name) )
					return elem.getAttributeNode( name ).nodeValue;

				// elem.tabIndex doesn't always return the correct value when it hasn't been explicitly set
				// http://fluidproject.org/blog/2008/01/09/getting-setting-and-removing-tabindex-values-with-javascript/
				if ( name == "tabIndex" ) {
					var attributeNode = elem.getAttributeNode( "tabIndex" );
					return attributeNode && attributeNode.specified
						? attributeNode.value
						: elem.nodeName.match(/(button|input|object|select|textarea)/i)
							? 0
							: elem.nodeName.match(/^(a|area)$/i) && elem.href
								? 0
								: undefined;
				}

				return elem[ name ];
			}

			if ( !jQuery.support.style && notxml &&  name == "style" )
				return jQuery.attr( elem.style, "cssText", value );

			if ( set )
				// convert the value to a string (all browsers do this but IE) see #1070
				elem.setAttribute( name, "" + value );

			var attr = !jQuery.support.hrefNormalized && notxml && special
					// Some attributes require a special call on IE
					? elem.getAttribute( name, 2 )
					: elem.getAttribute( name );

			// Non-existent attributes return null, we normalize to undefined
			return attr === null ? undefined : attr;
		}

		// elem is actually elem.style ... set the style

		// IE uses filters for opacity
		if ( !jQuery.support.opacity && name == "opacity" ) {
			if ( set ) {
				// IE has trouble with opacity if it does not have layout
				// Force it by setting the zoom level
				elem.zoom = 1;

				// Set the alpha filter to set the opacity
				elem.filter = (elem.filter || "").replace( /alpha\([^)]*\)/, "" ) +
					(parseInt( value ) + '' == "NaN" ? "" : "alpha(opacity=" + value * 100 + ")");
			}

			return elem.filter && elem.filter.indexOf("opacity=") >= 0 ?
				(parseFloat( elem.filter.match(/opacity=([^)]*)/)[1] ) / 100) + '':
				"";
		}

		name = name.replace(/-([a-z])/ig, function(all, letter){
			return letter.toUpperCase();
		});

		if ( set )
			elem[ name ] = value;

		return elem[ name ];
	},

	trim: function( text ) {
		///	<summary>
		///		Remove the whitespace from the beginning and end of a string.
		///		Part of JavaScript
		///	</summary>
		///	<returns type="String" />
		///	<param name="text" type="String">
		///		The string to trim.
		///	</param>

		return (text || "").replace( /^\s+|\s+$/g, "" );
	},

	makeArray: function( array ) {
		///	<summary>
		///		Turns anything into a true array.  This is an internal method.
		///	</summary>
		///	<param name="array" type="Object">Anything to turn into an actual Array</param>
		///	<returns type="Array" />
		///	<private />

		var ret = [];

		if( array != null ){
			var i = array.length;
			// The window, strings (and functions) also have 'length'
			if( i == null || typeof array === "string" || jQuery.isFunction(array) || array.setInterval )
				ret[0] = array;
			else
				while( i )
					ret[--i] = array[i];
		}

		return ret;
	},

	inArray: function( elem, array ) {
		///	<summary>
		///		Determines the index of the first parameter in the array.
		///	</summary>
		///	<param name="elem">The value to see if it exists in the array.</param>
		///	<param name="array" type="Array">The array to look through for the value</param>
		///	<returns type="Number" integer="true">The 0-based index of the item if it was found, otherwise -1.</returns>

		for ( var i = 0, length = array.length; i < length; i++ )
		// Use === because on IE, window == document
			if ( array[ i ] === elem )
				return i;

		return -1;
	},

	merge: function( first, second ) {
		///	<summary>
		///		Merge two arrays together, removing all duplicates.
		///		The new array is: All the results from the first array, followed
		///		by the unique results from the second array.
		///		Part of JavaScript
		///	</summary>
		///	<returns type="Array" />
		///	<param name="first" type="Array">
		///		 The first array to merge.
		///	</param>
		///	<param name="second" type="Array">
		///		 The second array to merge.
		///	</param>

		// We have to loop this way because IE & Opera overwrite the length
		// expando of getElementsByTagName
		var i = 0, elem, pos = first.length;
		// Also, we need to make sure that the correct elements are being returned
		// (IE returns comment nodes in a '*' query)
		if ( !jQuery.support.getAll ) {
			while ( (elem = second[ i++ ]) != null )
				if ( elem.nodeType != 8 )
					first[ pos++ ] = elem;

		} else
			while ( (elem = second[ i++ ]) != null )
				first[ pos++ ] = elem;

		return first;
	},

	unique: function( array ) {
		///	<summary>
		///		Removes all duplicate elements from an array of elements.
		///	</summary>
		///	<param name="array" type="Array&lt;Element&gt;">The array to translate</param>
		///	<returns type="Array&lt;Element&gt;">The array after translation.</returns>

		var ret = [], done = {};

		try {

			for ( var i = 0, length = array.length; i < length; i++ ) {
				var id = jQuery.data( array[ i ] );

				if ( !done[ id ] ) {
					done[ id ] = true;
					ret.push( array[ i ] );
				}
			}

		} catch( e ) {
			ret = array;
		}

		return ret;
	},

	grep: function( elems, callback, inv ) {
		///	<summary>
		///		Filter items out of an array, by using a filter function.
		///		The specified function will be passed two arguments: The
		///		current array item and the index of the item in the array. The
		///		function must return 'true' to keep the item in the array, 
		///		false to remove it.
		///		});
		///		Part of JavaScript
		///	</summary>
		///	<returns type="Array" />
		///	<param name="elems" type="Array">
		///		array The Array to find items in.
		///	</param>
		///	<param name="fn" type="Function">
		///		 The function to process each item against.
		///	</param>
		///	<param name="inv" type="Boolean">
		///		 Invert the selection - select the opposite of the function.
		///	</param>
		
		var ret = [];

		// Go through the array, only saving the items
		// that pass the validator function
		for ( var i = 0, length = elems.length; i < length; i++ )
			if ( !inv != !callback( elems[ i ], i ) )
				ret.push( elems[ i ] );

		return ret;
	},

	map: function( elems, callback ) {
		///	<summary>
		///		Translate all items in an array to another array of items.
		///		The translation function that is provided to this method is 
		///		called for each item in the array and is passed one argument: 
		///		The item to be translated.
		///		The function can then return the translated value, 'null'
		///		(to remove the item), or  an array of values - which will
		///		be flattened into the full array.
		///		Part of JavaScript
		///	</summary>
		///	<returns type="Array" />
		///	<param name="elems" type="Array">
		///		array The Array to translate.
		///	</param>
		///	<param name="fn" type="Function">
		///		 The function to process each item against.
		///	</param>
		
		var ret = [];

		// Go through the array, translating each of the items to their
		// new value (or values).
		for ( var i = 0, length = elems.length; i < length; i++ ) {
			var value = callback( elems[ i ], i );

			if ( value != null )
				ret[ ret.length ] = value;
		}

		return ret.concat.apply( [], ret );
	}
});

// Use of jQuery.browser is deprecated.
// It's included for backwards compatibility and plugins,
// although they should work to migrate away.

var userAgent = navigator.userAgent.toLowerCase();

// Figure out what browser is being used
jQuery.browser = {
	version: (userAgent.match( /.+(?:rv|it|ra|ie)[\/: ]([\d.]+)/ ) || [0,'0'])[1],
	safari: /webkit/.test( userAgent ),
	opera: /opera/.test( userAgent ),
	msie: /msie/.test( userAgent ) && !/opera/.test( userAgent ),
	mozilla: /mozilla/.test( userAgent ) && !/(compatible|webkit)/.test( userAgent )
};

// [vsdoc] The following section has been denormalized from original sources for IntelliSense.
// jQuery.each({
// 	parent: function(elem){return elem.parentNode;},
// 	parents: function(elem){return jQuery.dir(elem,"parentNode");},
// 	next: function(elem){return jQuery.nth(elem,2,"nextSibling");},
// 	prev: function(elem){return jQuery.nth(elem,2,"previousSibling");},
// 	nextAll: function(elem){return jQuery.dir(elem,"nextSibling");},
// 	prevAll: function(elem){return jQuery.dir(elem,"previousSibling");},
// 	siblings: function(elem){return jQuery.sibling(elem.parentNode.firstChild,elem);},
// 	children: function(elem){return jQuery.sibling(elem.firstChild);},
// 	contents: function(elem){return jQuery.nodeName(elem,"iframe")?elem.contentDocument||elem.contentWindow.document:jQuery.makeArray(elem.childNodes);}
// }, function(name, fn){
// 	jQuery.fn[ name ] = function( selector ) {
// 		///	<summary>
// 		///		Get a set of elements containing the unique parents of the matched
// 		///		set of elements.
// 		///		Can be filtered with an optional expressions.
// 		///		Part of DOM/Traversing
// 		///	</summary>
// 		///	<param name="expr" type="String" optional="true">
// 		///		(optional) An expression to filter the parents with
// 		///	</param>
// 		///	<returns type="jQuery" />   
// 		
// 		var ret = jQuery.map( this, fn );
// 
// 		if ( selector && typeof selector == "string" )
// 			ret = jQuery.multiFilter( selector, ret );
// 
// 		return this.pushStack( jQuery.unique( ret ), name, selector );
// 	};
// });

jQuery.each({
	parent: function(elem){return elem.parentNode;}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>
		///		Get a set of elements containing the unique parents of the matched
		///		set of elements.
		///		Can be filtered with an optional expressions.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the parents with
		///	</param>
		///	<returns type="jQuery" />   
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	parents: function(elem){return jQuery.dir(elem,"parentNode");}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>
		///		Get a set of elements containing the unique ancestors of the matched
		///		set of elements (except for the root element).
		///		Can be filtered with an optional expressions.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the ancestors with
		///	</param>
		///	<returns type="jQuery" />   
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	next: function(elem){return jQuery.nth(elem,2,"nextSibling");}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>
		///		Get a set of elements containing the unique next siblings of each of the
		///		matched set of elements.
		///		It only returns the very next sibling, not all next siblings.
		///		Can be filtered with an optional expressions.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the next Elements with
		///	</param>
		///	<returns type="jQuery" />
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	prev: function(elem){return jQuery.nth(elem,2,"previousSibling");}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>
		///		Get a set of elements containing the unique previous siblings of each of the
		///		matched set of elements.
		///		Can be filtered with an optional expressions.
		///		It only returns the immediately previous sibling, not all previous siblings.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the previous Elements with
		///	</param>
		///	<returns type="jQuery" />
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	nextAll: function(elem){return jQuery.dir(elem,"nextSibling");}
}, function(name, fn){
	jQuery.fn[name] = function(selector) {
		///	<summary>
		///		Finds all sibling elements after the current element.
		///		Can be filtered with an optional expressions.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the next Elements with
		///	</param>
		///	<returns type="jQuery" />
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	prevAll: function(elem){return jQuery.dir(elem,"previousSibling");}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>
		///		Finds all sibling elements before the current element.
		///		Can be filtered with an optional expressions.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the previous Elements with
		///	</param>
		///	<returns type="jQuery" />
	
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	siblings: function(elem){return jQuery.sibling(elem.parentNode.firstChild,elem);}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>
		///		Get a set of elements containing all of the unique siblings of each of the
		///		matched set of elements.
		///		Can be filtered with an optional expressions.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the sibling Elements with
		///	</param>
		///	<returns type="jQuery" />
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	children: function(elem){return jQuery.sibling(elem.firstChild);}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>
		///		Get a set of elements containing all of the unique children of each of the
		///		matched set of elements.
		///		Can be filtered with an optional expressions.
		///		Part of DOM/Traversing
		///	</summary>
		///	<param name="expr" type="String" optional="true">
		///		(optional) An expression to filter the child Elements with
		///	</param>
		///	<returns type="jQuery" />
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

jQuery.each({
	contents: function(elem){return jQuery.nodeName(elem,"iframe")?elem.contentDocument||elem.contentWindow.document:jQuery.makeArray(elem.childNodes);}
}, function(name, fn){
	jQuery.fn[ name ] = function( selector ) {
		///	<summary>Finds all the child nodes inside the matched elements including text nodes, or the content document if the element is an iframe.</summary>
		///	<returns type="jQuery" />
		
		var ret = jQuery.map( this, fn );

		if ( selector && typeof selector == "string" )
			ret = jQuery.multiFilter( selector, ret );

		return this.pushStack( jQuery.unique( ret ), name, selector );
	};
});

// [vsdoc] The following section has been denormalized from original sources for IntelliSense.
// jQuery.each({
// 	appendTo: "append",
// 	prependTo: "prepend",
// 	insertBefore: "before",
// 	insertAfter: "after",
// 	replaceAll: "replaceWith"
// }, function(name, original){
// 	jQuery.fn[ name ] = function() {
// 		var args = arguments;
// 
// 		return this.each(function(){
// 			for ( var i = 0, length = args.length; i < length; i++ )
// 				jQuery( args[ i ] )[ original ]( this );
// 		});
// 	};
// });

jQuery.fn.appendTo = function( selector ) {
	///	<summary>
	///		Append all of the matched elements to another, specified, set of elements.
	///		As of jQuery 1.3.2, returns all of the inserted elements.
	///		This operation is, essentially, the reverse of doing a regular
	///		$(A).append(B), in that instead of appending B to A, you're appending
	///		A to B.
	///	</summary>
	///	<param name="selector" type="Selector">
	///		 target to which the content will be appended.
	///	</param>
	///	<returns type="jQuery" />
	var ret = [], insert = jQuery( selector );

	for ( var i = 0, l = insert.length; i < l; i++ ) {
		var elems = (i > 0 ? this.clone(true) : this).get();
		jQuery.fn[ "append" ].apply( jQuery(insert[i]), elems );
		ret = ret.concat( elems );
	}

	return this.pushStack( ret, "appendTo", selector );
};

jQuery.fn.prependTo = function( selector ) {
	///	<summary>
	///		Prepend all of the matched elements to another, specified, set of elements.
	///		As of jQuery 1.3.2, returns all of the inserted elements.
	///		This operation is, essentially, the reverse of doing a regular
	///		$(A).prepend(B), in that instead of prepending B to A, you're prepending
	///		A to B.
	///	</summary>
	///	<param name="selector" type="Selector">
	///		 target to which the content will be appended.
	///	</param>
	///	<returns type="jQuery" />
	var ret = [], insert = jQuery( selector );

	for ( var i = 0, l = insert.length; i < l; i++ ) {
		var elems = (i > 0 ? this.clone(true) : this).get();
		jQuery.fn[ "prepend" ].apply( jQuery(insert[i]), elems );
		ret = ret.concat( elems );
	}

	return this.pushStack( ret, "prependTo", selector );
};

jQuery.fn.insertBefore = function( selector ) {
	///	<summary>
	///		Insert all of the matched elements before another, specified, set of elements.
	///		As of jQuery 1.3.2, returns all of the inserted elements.
	///		This operation is, essentially, the reverse of doing a regular
	///		$(A).before(B), in that instead of inserting B before A, you're inserting
	///		A before B.
	///	</summary>
	///	<param name="content" type="String">
	///		 Content after which the selected element(s) is inserted.
	///	</param>
	///	<returns type="jQuery" />
	var ret = [], insert = jQuery( selector );

	for ( var i = 0, l = insert.length; i < l; i++ ) {
		var elems = (i > 0 ? this.clone(true) : this).get();
		jQuery.fn[ "before" ].apply( jQuery(insert[i]), elems );
		ret = ret.concat( elems );
	}

	return this.pushStack( ret, "insertBefore", selector );
};

jQuery.fn.insertAfter = function( selector ) {
	///	<summary>
	///		Insert all of the matched elements after another, specified, set of elements.
	///		As of jQuery 1.3.2, returns all of the inserted elements.
	///		This operation is, essentially, the reverse of doing a regular
	///		$(A).after(B), in that instead of inserting B after A, you're inserting
	///		A after B.
	///	</summary>
	///	<param name="content" type="String">
	///		 Content after which the selected element(s) is inserted.
	///	</param>
	///	<returns type="jQuery" />
	var ret = [], insert = jQuery( selector );

	for ( var i = 0, l = insert.length; i < l; i++ ) {
		var elems = (i > 0 ? this.clone(true) : this).get();
		jQuery.fn[ "after" ].apply( jQuery(insert[i]), elems );
		ret = ret.concat( elems );
	}

	return this.pushStack( ret, "insertAfter", selector );
};

jQuery.fn.replaceAll = function( selector ) {
	///	<summary>
	///		Replaces the elements matched by the specified selector with the matched elements.
	///		As of jQuery 1.3.2, returns all of the inserted elements.
	///	</summary>
	///	<param name="selector" type="Selector">The elements to find and replace the matched elements with.</param>
	///	<returns type="jQuery" />
	var ret = [], insert = jQuery( selector );

	for ( var i = 0, l = insert.length; i < l; i++ ) {
		var elems = (i > 0 ? this.clone(true) : this).get();
		jQuery.fn[ "replaceWith" ].apply( jQuery(insert[i]), elems );
		ret = ret.concat( elems );
	}

	return this.pushStack( ret, "replaceAll", selector );
};

// [vsdoc] The following section has been denormalized from original sources for IntelliSense.
// jQuery.each({
// 	removeAttr: function( name ) {
// 		jQuery.attr( this, name, "" );
// 		if (this.nodeType == 1)
// 			this.removeAttribute( name );
// 	},
// 
// 	addClass: function( classNames ) {
// 		jQuery.className.add( this, classNames );
// 	},
// 
// 	removeClass: function( classNames ) {
// 		jQuery.className.remove( this, classNames );
// 	},
// 
// 	toggleClass: function( classNames, state ) {
// 		if( typeof state !== "boolean" )
// 			state = !jQuery.className.has( this, classNames );
// 		jQuery.className[ state ? "add" : "remove" ]( this, classNames );
// 	},
// 
// 	remove: function( selector ) {
// 		if ( !selector || jQuery.filter( selector, [ this ] ).length ) {
// 			// Prevent memory leaks
// 			jQuery( "*", this ).add([this]).each(function(){
// 				jQuery.event.remove(this);
// 				jQuery.removeData(this);
// 			});
// 			if (this.parentNode)
// 				this.parentNode.removeChild( this );
// 		}
// 	},
// 
// 	empty: function() {
// 		// Remove element nodes and prevent memory leaks
// 		jQuery( ">*", this ).remove();
// 
// 		// Remove any remaining nodes
// 		while ( this.firstChild )
// 			this.removeChild( this.firstChild );
// 	}
// }, function(name, fn){
// 	jQuery.fn[ name ] = function(){
// 		return this.each( fn, arguments );
// 	};
// });

jQuery.fn.removeAttr = function(){
	///	<summary>
	///		Remove an attribute from each of the matched elements.
	///		Part of DOM/Attributes
	///	</summary>
	///	<param name="key" type="String">
	///		name The name of the attribute to remove.
	///	</param>
	///	<returns type="jQuery" />
	return this.each( function( name ) {
		jQuery.attr( this, name, "" );
		if (this.nodeType == 1)
			this.removeAttribute( name );
	}, arguments );
};

jQuery.fn.addClass = function(){
	///	<summary>
	///		Adds the specified class(es) to each of the set of matched elements.
	///		Part of DOM/Attributes
	///	</summary>
	///	<param name="classNames" type="String">
	///		lass One or more CSS classes to add to the elements
	///	</param>
	///	<returns type="jQuery" />
	return this.each( function( classNames ) {
		jQuery.className.add( this, classNames );
	}, arguments );
};

jQuery.fn.removeClass = function(){
	///	<summary>
	///		Removes all or the specified class(es) from the set of matched elements.
	///		Part of DOM/Attributes
	///	</summary>
	///	<param name="cssClasses" type="String" optional="true">
	///		(Optional) One or more CSS classes to remove from the elements
	///	</param>
	///	<returns type="jQuery" />
	return this.each( function( classNames ) {
		jQuery.className.remove( this, classNames );
	}, arguments );
};

jQuery.fn.toggleClass = function(){
	///	<summary>
	///		Adds the specified class if it is not present, removes it if it is
	///		present.
	///		Part of DOM/Attributes
	///	</summary>
	///	<param name="cssClass" type="String">
	///		A CSS class with which to toggle the elements
	///	</param>
	///	<returns type="jQuery" />
	return this.each( function( classNames, state ) {
		if( typeof state !== "boolean" )
			state = !jQuery.className.has( this, classNames );
		jQuery.className[ state ? "add" : "remove" ]( this, classNames );
	}, arguments );
};

jQuery.fn.remove = function(){
	///	<summary>
	///		Removes all matched elements from the DOM. This does NOT remove them from the
	///		jQuery object, allowing you to use the matched elements further.
	///		Can be filtered with an optional expressions.
	///		Part of DOM/Manipulation
	///	</summary>
	///	<param name="expr" type="String" optional="true">
	///		(optional) A jQuery expression to filter elements by.
	///	</param>
	///	<returns type="jQuery" />
	return this.each( function( selector ) {
		if ( !selector || jQuery.filter( selector, [ this ] ).length ) {
			// Prevent memory leaks
			jQuery( "*", this ).add([this]).each(function(){
				jQuery.event.remove(this);
				jQuery.removeData(this);
			});
			if (this.parentNode)
				this.parentNode.removeChild( this );
		}
	}, arguments );
};

jQuery.fn.empty = function(){
	///	<summary>
	///		Removes all child nodes from the set of matched elements.
	///		Part of DOM/Manipulation
	///	</summary>
	///	<returns type="jQuery" />
	return this.each( function() {
		// Remove element nodes and prevent memory leaks
		jQuery(this).children().remove();

		// Remove any remaining nodes
		while ( this.firstChild )
			this.removeChild( this.firstChild );
	}, arguments );
};

// Helper function used by the dimensions and offset modules
function num(elem, prop) {
	return elem[0] && parseInt( jQuery.curCSS(elem[0], prop, true), 10 ) || 0;
}
var expando = "jQuery" + now(), uuid = 0, windowData = {};

jQuery.extend({
	cache: {},

	data: function( elem, name, data ) {
		elem = elem == window ?
			windowData :
			elem;

		var id = elem[ expando ];

		// Compute a unique ID for the element
		if ( !id )
			id = elem[ expando ] = ++uuid;

		// Only generate the data cache if we're
		// trying to access or manipulate it
		if ( name && !jQuery.cache[ id ] )
			jQuery.cache[ id ] = {};

		// Prevent overriding the named cache with undefined values
		if ( data !== undefined )
			jQuery.cache[ id ][ name ] = data;

		// Return the named cache data, or the ID for the element
		return name ?
			jQuery.cache[ id ][ name ] :
			id;
	},

	removeData: function( elem, name ) {
		elem = elem == window ?
			windowData :
			elem;

		var id = elem[ expando ];

		// If we want to remove a specific section of the element's data
		if ( name ) {
			if ( jQuery.cache[ id ] ) {
				// Remove the section of cache data
				delete jQuery.cache[ id ][ name ];

				// If we've removed all the data, remove the element's cache
				name = "";

				for ( name in jQuery.cache[ id ] )
					break;

				if ( !name )
					jQuery.removeData( elem );
			}

		// Otherwise, we want to remove all of the element's data
		} else {
			// Clean up the element expando
			try {
				delete elem[ expando ];
			} catch(e){
				// IE has trouble directly removing the expando
				// but it's ok with using removeAttribute
				if ( elem.removeAttribute )
					elem.removeAttribute( expando );
			}

			// Completely remove the data cache
			delete jQuery.cache[ id ];
		}
	},
	queue: function( elem, type, data ) {
		if ( elem ){
	
			type = (type || "fx") + "queue";
	
			var q = jQuery.data( elem, type );
	
			if ( !q || jQuery.isArray(data) )
				q = jQuery.data( elem, type, jQuery.makeArray(data) );
			else if( data )
				q.push( data );
	
		}
		return q;
	},

	dequeue: function( elem, type ){
		var queue = jQuery.queue( elem, type ),
			fn = queue.shift();
		
		if( !type || type === "fx" )
			fn = queue[0];
			
		if( fn !== undefined )
			fn.call(elem);
	}
});

jQuery.fn.extend({
	data: function( key, value ){
		var parts = key.split(".");
		parts[1] = parts[1] ? "." + parts[1] : "";

		if ( value === undefined ) {
			var data = this.triggerHandler("getData" + parts[1] + "!", [parts[0]]);

			if ( data === undefined && this.length )
				data = jQuery.data( this[0], key );

			return data === undefined && parts[1] ?
				this.data( parts[0] ) :
				data;
		} else
			return this.trigger("setData" + parts[1] + "!", [parts[0], value]).each(function(){
				jQuery.data( this, key, value );
			});
	},

	removeData: function( key ){
		return this.each(function(){
			jQuery.removeData( this, key );
		});
	},
	queue: function(type, data){
		///	<summary>
		///		1: queue() - Returns a reference to the first element's queue (which is an array of functions).
		///		2: queue(callback) - Adds a new function, to be executed, onto the end of the queue of all matched elements.
		///		3: queue(queue) - Replaces the queue of all matched element with this new queue (the array of functions).
		///	</summary>
		///	<param name="type" type="Function">The function to add to the queue.</param>
		///	<returns type="jQuery" />

		if ( typeof type !== "string" ) {
			data = type;
			type = "fx";
		}

		if ( data === undefined )
			return jQuery.queue( this[0], type );

		return this.each(function(){
			var queue = jQuery.queue( this, type, data );
			
			 if( type == "fx" && queue.length == 1 )
				queue[0].call(this);
		});
	},
	dequeue: function(type){
		///	<summary>
		///		Removes a queued function from the front of the queue and executes it.
		///	</summary>
		///	<param name="type" type="String" optional="true">The type of queue to access.</param>
		///	<returns type="jQuery" />
		
		return this.each(function(){
			jQuery.dequeue( this, type );
		});
	}
});/*!
 * Sizzle CSS Selector Engine - v0.9.3
 *  Copyright 2009, The Dojo Foundation
 *  More information: http://sizzlejs.com/
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 */
(function(){

var chunker = /((?:\((?:\([^()]+\)|[^()]+)+\)|\[(?:\[[^[\]]*\]|['"][^'"]*['"]|[^[\]'"]+)+\]|\\.|[^ >+~,(\[\\]+)+|[>+~])(\s*,\s*)?/g,
	done = 0,
	toString = Object.prototype.toString;

var Sizzle = function(selector, context, results, seed) {
	results = results || [];
	context = context || document;

	if ( context.nodeType !== 1 && context.nodeType !== 9 )
		return [];
	
	if ( !selector || typeof selector !== "string" ) {
		return results;
	}

	var parts = [], m, set, checkSet, check, mode, extra, prune = true;
	
	// Reset the position of the chunker regexp (start from head)
	chunker.lastIndex = 0;
	
	while ( (m = chunker.exec(selector)) !== null ) {
		parts.push( m[1] );
		
		if ( m[2] ) {
			extra = RegExp.rightContext;
			break;
		}
	}

	if ( parts.length > 1 && origPOS.exec( selector ) ) {
		if ( parts.length === 2 && Expr.relative[ parts[0] ] ) {
			set = posProcess( parts[0] + parts[1], context );
		} else {
			set = Expr.relative[ parts[0] ] ?
				[ context ] :
				Sizzle( parts.shift(), context );

			while ( parts.length ) {
				selector = parts.shift();

				if ( Expr.relative[ selector ] )
					selector += parts.shift();

				set = posProcess( selector, set );
			}
		}
	} else {
		var ret = seed ?
			{ expr: parts.pop(), set: makeArray(seed) } :
			Sizzle.find( parts.pop(), parts.length === 1 && context.parentNode ? context.parentNode : context, isXML(context) );
		set = Sizzle.filter( ret.expr, ret.set );

		if ( parts.length > 0 ) {
			checkSet = makeArray(set);
		} else {
			prune = false;
		}

		while ( parts.length ) {
			var cur = parts.pop(), pop = cur;

			if ( !Expr.relative[ cur ] ) {
				cur = "";
			} else {
				pop = parts.pop();
			}

			if ( pop == null ) {
				pop = context;
			}

			Expr.relative[ cur ]( checkSet, pop, isXML(context) );
		}
	}

	if ( !checkSet ) {
		checkSet = set;
	}

	if ( !checkSet ) {
		throw "Syntax error, unrecognized expression: " + (cur || selector);
	}

	if ( toString.call(checkSet) === "[object Array]" ) {
		if ( !prune ) {
			results.push.apply( results, checkSet );
		} else if ( context.nodeType === 1 ) {
			for ( var i = 0; checkSet[i] != null; i++ ) {
				if ( checkSet[i] && (checkSet[i] === true || checkSet[i].nodeType === 1 && contains(context, checkSet[i])) ) {
					results.push( set[i] );
				}
			}
		} else {
			for ( var i = 0; checkSet[i] != null; i++ ) {
				if ( checkSet[i] && checkSet[i].nodeType === 1 ) {
					results.push( set[i] );
				}
			}
		}
	} else {
		makeArray( checkSet, results );
	}

	if ( extra ) {
		Sizzle( extra, context, results, seed );

		if ( sortOrder ) {
			hasDuplicate = false;
			results.sort(sortOrder);

			if ( hasDuplicate ) {
				for ( var i = 1; i < results.length; i++ ) {
					if ( results[i] === results[i-1] ) {
						results.splice(i--, 1);
					}
				}
			}
		}
	}

	return results;
};

Sizzle.matches = function(expr, set){
	return Sizzle(expr, null, null, set);
};

Sizzle.find = function(expr, context, isXML){
	var set, match;

	if ( !expr ) {
		return [];
	}

	for ( var i = 0, l = Expr.order.length; i < l; i++ ) {
		var type = Expr.order[i], match;
		
		if ( (match = Expr.match[ type ].exec( expr )) ) {
			var left = RegExp.leftContext;

			if ( left.substr( left.length - 1 ) !== "\\" ) {
				match[1] = (match[1] || "").replace(/\\/g, "");
				set = Expr.find[ type ]( match, context, isXML );
				if ( set != null ) {
					expr = expr.replace( Expr.match[ type ], "" );
					break;
				}
			}
		}
	}

	if ( !set ) {
		set = context.getElementsByTagName("*");
	}

	return {set: set, expr: expr};
};

Sizzle.filter = function(expr, set, inplace, not){
	var old = expr, result = [], curLoop = set, match, anyFound,
		isXMLFilter = set && set[0] && isXML(set[0]);

	while ( expr && set.length ) {
		for ( var type in Expr.filter ) {
			if ( (match = Expr.match[ type ].exec( expr )) != null ) {
				var filter = Expr.filter[ type ], found, item;
				anyFound = false;

				if ( curLoop == result ) {
					result = [];
				}

				if ( Expr.preFilter[ type ] ) {
					match = Expr.preFilter[ type ]( match, curLoop, inplace, result, not, isXMLFilter );

					if ( !match ) {
						anyFound = found = true;
					} else if ( match === true ) {
						continue;
					}
				}

				if ( match ) {
					for ( var i = 0; (item = curLoop[i]) != null; i++ ) {
						if ( item ) {
							found = filter( item, match, i, curLoop );
							var pass = not ^ !!found;

							if ( inplace && found != null ) {
								if ( pass ) {
									anyFound = true;
								} else {
									curLoop[i] = false;
								}
							} else if ( pass ) {
								result.push( item );
								anyFound = true;
							}
						}
					}
				}

				if ( found !== undefined ) {
					if ( !inplace ) {
						curLoop = result;
					}

					expr = expr.replace( Expr.match[ type ], "" );

					if ( !anyFound ) {
						return [];
					}

					break;
				}
			}
		}

		// Improper expression
		if ( expr == old ) {
			if ( anyFound == null ) {
				throw "Syntax error, unrecognized expression: " + expr;
			} else {
				break;
			}
		}

		old = expr;
	}

	return curLoop;
};

var Expr = Sizzle.selectors = {
	order: [ "ID", "NAME", "TAG" ],
	match: {
		ID: /#((?:[\w\u00c0-\uFFFF_-]|\\.)+)/,
		CLASS: /\.((?:[\w\u00c0-\uFFFF_-]|\\.)+)/,
		NAME: /\[name=['"]*((?:[\w\u00c0-\uFFFF_-]|\\.)+)['"]*\]/,
		ATTR: /\[\s*((?:[\w\u00c0-\uFFFF_-]|\\.)+)\s*(?:(\S?=)\s*(['"]*)(.*?)\3|)\s*\]/,
		TAG: /^((?:[\w\u00c0-\uFFFF\*_-]|\\.)+)/,
		CHILD: /:(only|nth|last|first)-child(?:\((even|odd|[\dn+-]*)\))?/,
		POS: /:(nth|eq|gt|lt|first|last|even|odd)(?:\((\d*)\))?(?=[^-]|$)/,
		PSEUDO: /:((?:[\w\u00c0-\uFFFF_-]|\\.)+)(?:\((['"]*)((?:\([^\)]+\)|[^\2\(\)]*)+)\2\))?/
	},
	attrMap: {
		"class": "className",
		"for": "htmlFor"
	},
	attrHandle: {
		href: function(elem){
			return elem.getAttribute("href");
		}
	},
	relative: {
		"+": function(checkSet, part, isXML){
			var isPartStr = typeof part === "string",
				isTag = isPartStr && !/\W/.test(part),
				isPartStrNotTag = isPartStr && !isTag;

			if ( isTag && !isXML ) {
				part = part.toUpperCase();
			}

			for ( var i = 0, l = checkSet.length, elem; i < l; i++ ) {
				if ( (elem = checkSet[i]) ) {
					while ( (elem = elem.previousSibling) && elem.nodeType !== 1 ) {}

					checkSet[i] = isPartStrNotTag || elem && elem.nodeName === part ?
						elem || false :
						elem === part;
				}
			}

			if ( isPartStrNotTag ) {
				Sizzle.filter( part, checkSet, true );
			}
		},
		">": function(checkSet, part, isXML){
			var isPartStr = typeof part === "string";

			if ( isPartStr && !/\W/.test(part) ) {
				part = isXML ? part : part.toUpperCase();

				for ( var i = 0, l = checkSet.length; i < l; i++ ) {
					var elem = checkSet[i];
					if ( elem ) {
						var parent = elem.parentNode;
						checkSet[i] = parent.nodeName === part ? parent : false;
					}
				}
			} else {
				for ( var i = 0, l = checkSet.length; i < l; i++ ) {
					var elem = checkSet[i];
					if ( elem ) {
						checkSet[i] = isPartStr ?
							elem.parentNode :
							elem.parentNode === part;
					}
				}

				if ( isPartStr ) {
					Sizzle.filter( part, checkSet, true );
				}
			}
		},
		"": function(checkSet, part, isXML){
			var doneName = done++, checkFn = dirCheck;

			if ( !part.match(/\W/) ) {
				var nodeCheck = part = isXML ? part : part.toUpperCase();
				checkFn = dirNodeCheck;
			}

			checkFn("parentNode", part, doneName, checkSet, nodeCheck, isXML);
		},
		"~": function(checkSet, part, isXML){
			var doneName = done++, checkFn = dirCheck;

			if ( typeof part === "string" && !part.match(/\W/) ) {
				var nodeCheck = part = isXML ? part : part.toUpperCase();
				checkFn = dirNodeCheck;
			}

			checkFn("previousSibling", part, doneName, checkSet, nodeCheck, isXML);
		}
	},
	find: {
		ID: function(match, context, isXML){
			if ( typeof context.getElementById !== "undefined" && !isXML ) {
				var m = context.getElementById(match[1]);
				return m ? [m] : [];
			}
		},
		NAME: function(match, context, isXML){
			if ( typeof context.getElementsByName !== "undefined" ) {
				var ret = [], results = context.getElementsByName(match[1]);

				for ( var i = 0, l = results.length; i < l; i++ ) {
					if ( results[i].getAttribute("name") === match[1] ) {
						ret.push( results[i] );
					}
				}

				return ret.length === 0 ? null : ret;
			}
		},
		TAG: function(match, context){
			return context.getElementsByTagName(match[1]);
		}
	},
	preFilter: {
		CLASS: function(match, curLoop, inplace, result, not, isXML){
			match = " " + match[1].replace(/\\/g, "") + " ";

			if ( isXML ) {
				return match;
			}

			for ( var i = 0, elem; (elem = curLoop[i]) != null; i++ ) {
				if ( elem ) {
					if ( not ^ (elem.className && (" " + elem.className + " ").indexOf(match) >= 0) ) {
						if ( !inplace )
							result.push( elem );
					} else if ( inplace ) {
						curLoop[i] = false;
					}
				}
			}

			return false;
		},
		ID: function(match){
			return match[1].replace(/\\/g, "");
		},
		TAG: function(match, curLoop){
			for ( var i = 0; curLoop[i] === false; i++ ){}
			return curLoop[i] && isXML(curLoop[i]) ? match[1] : match[1].toUpperCase();
		},
		CHILD: function(match){
			if ( match[1] == "nth" ) {
				// parse equations like 'even', 'odd', '5', '2n', '3n+2', '4n-1', '-n+6'
				var test = /(-?)(\d*)n((?:\+|-)?\d*)/.exec(
					match[2] == "even" && "2n" || match[2] == "odd" && "2n+1" ||
					!/\D/.test( match[2] ) && "0n+" + match[2] || match[2]);

				// calculate the numbers (first)n+(last) including if they are negative
				match[2] = (test[1] + (test[2] || 1)) - 0;
				match[3] = test[3] - 0;
			}

			// TODO: Move to normal caching system
			match[0] = done++;

			return match;
		},
		ATTR: function(match, curLoop, inplace, result, not, isXML){
			var name = match[1].replace(/\\/g, "");
			
			if ( !isXML && Expr.attrMap[name] ) {
				match[1] = Expr.attrMap[name];
			}

			if ( match[2] === "~=" ) {
				match[4] = " " + match[4] + " ";
			}

			return match;
		},
		PSEUDO: function(match, curLoop, inplace, result, not){
			if ( match[1] === "not" ) {
				// If we're dealing with a complex expression, or a simple one
				if ( match[3].match(chunker).length > 1 || /^\w/.test(match[3]) ) {
					match[3] = Sizzle(match[3], null, null, curLoop);
				} else {
					var ret = Sizzle.filter(match[3], curLoop, inplace, true ^ not);
					if ( !inplace ) {
						result.push.apply( result, ret );
					}
					return false;
				}
			} else if ( Expr.match.POS.test( match[0] ) || Expr.match.CHILD.test( match[0] ) ) {
				return true;
			}
			
			return match;
		},
		POS: function(match){
			match.unshift( true );
			return match;
		}
	},
	filters: {
		enabled: function(elem){
			return elem.disabled === false && elem.type !== "hidden";
		},
		disabled: function(elem){
			return elem.disabled === true;
		},
		checked: function(elem){
			return elem.checked === true;
		},
		selected: function(elem){
			// Accessing this property makes selected-by-default
			// options in Safari work properly
			elem.parentNode.selectedIndex;
			return elem.selected === true;
		},
		parent: function(elem){
			return !!elem.firstChild;
		},
		empty: function(elem){
			return !elem.firstChild;
		},
		has: function(elem, i, match){
			return !!Sizzle( match[3], elem ).length;
		},
		header: function(elem){
			return /h\d/i.test( elem.nodeName );
		},
		text: function(elem){
			return "text" === elem.type;
		},
		radio: function(elem){
			return "radio" === elem.type;
		},
		checkbox: function(elem){
			return "checkbox" === elem.type;
		},
		file: function(elem){
			return "file" === elem.type;
		},
		password: function(elem){
			return "password" === elem.type;
		},
		submit: function(elem){
			return "submit" === elem.type;
		},
		image: function(elem){
			return "image" === elem.type;
		},
		reset: function(elem){
			return "reset" === elem.type;
		},
		button: function(elem){
			return "button" === elem.type || elem.nodeName.toUpperCase() === "BUTTON";
		},
		input: function(elem){
			return /input|select|textarea|button/i.test(elem.nodeName);
		}
	},
	setFilters: {
		first: function(elem, i){
			return i === 0;
		},
		last: function(elem, i, match, array){
			return i === array.length - 1;
		},
		even: function(elem, i){
			return i % 2 === 0;
		},
		odd: function(elem, i){
			return i % 2 === 1;
		},
		lt: function(elem, i, match){
			return i < match[3] - 0;
		},
		gt: function(elem, i, match){
			return i > match[3] - 0;
		},
		nth: function(elem, i, match){
			return match[3] - 0 == i;
		},
		eq: function(elem, i, match){
			return match[3] - 0 == i;
		}
	},
	filter: {
		PSEUDO: function(elem, match, i, array){
			var name = match[1], filter = Expr.filters[ name ];

			if ( filter ) {
				return filter( elem, i, match, array );
			} else if ( name === "contains" ) {
				return (elem.textContent || elem.innerText || "").indexOf(match[3]) >= 0;
			} else if ( name === "not" ) {
				var not = match[3];

				for ( var i = 0, l = not.length; i < l; i++ ) {
					if ( not[i] === elem ) {
						return false;
					}
				}

				return true;
			}
		},
		CHILD: function(elem, match){
			var type = match[1], node = elem;
			switch (type) {
				case 'only':
				case 'first':
					while (node = node.previousSibling)  {
						if ( node.nodeType === 1 ) return false;
					}
					if ( type == 'first') return true;
					node = elem;
				case 'last':
					while (node = node.nextSibling)  {
						if ( node.nodeType === 1 ) return false;
					}
					return true;
				case 'nth':
					var first = match[2], last = match[3];

					if ( first == 1 && last == 0 ) {
						return true;
					}
					
					var doneName = match[0],
						parent = elem.parentNode;
	
					if ( parent && (parent.sizcache !== doneName || !elem.nodeIndex) ) {
						var count = 0;
						for ( node = parent.firstChild; node; node = node.nextSibling ) {
							if ( node.nodeType === 1 ) {
								node.nodeIndex = ++count;
							}
						} 
						parent.sizcache = doneName;
					}
					
					var diff = elem.nodeIndex - last;
					if ( first == 0 ) {
						return diff == 0;
					} else {
						return ( diff % first == 0 && diff / first >= 0 );
					}
			}
		},
		ID: function(elem, match){
			return elem.nodeType === 1 && elem.getAttribute("id") === match;
		},
		TAG: function(elem, match){
			return (match === "*" && elem.nodeType === 1) || elem.nodeName === match;
		},
		CLASS: function(elem, match){
			return (" " + (elem.className || elem.getAttribute("class")) + " ")
				.indexOf( match ) > -1;
		},
		ATTR: function(elem, match){
			var name = match[1],
				result = Expr.attrHandle[ name ] ?
					Expr.attrHandle[ name ]( elem ) :
					elem[ name ] != null ?
						elem[ name ] :
						elem.getAttribute( name ),
				value = result + "",
				type = match[2],
				check = match[4];

			return result == null ?
				type === "!=" :
				type === "=" ?
				value === check :
				type === "*=" ?
				value.indexOf(check) >= 0 :
				type === "~=" ?
				(" " + value + " ").indexOf(check) >= 0 :
				!check ?
				value && result !== false :
				type === "!=" ?
				value != check :
				type === "^=" ?
				value.indexOf(check) === 0 :
				type === "$=" ?
				value.substr(value.length - check.length) === check :
				type === "|=" ?
				value === check || value.substr(0, check.length + 1) === check + "-" :
				false;
		},
		POS: function(elem, match, i, array){
			var name = match[2], filter = Expr.setFilters[ name ];

			if ( filter ) {
				return filter( elem, i, match, array );
			}
		}
	}
};

var origPOS = Expr.match.POS;

for ( var type in Expr.match ) {
	Expr.match[ type ] = RegExp( Expr.match[ type ].source + /(?![^\[]*\])(?![^\(]*\))/.source );
}

var makeArray = function(array, results) {
	array = Array.prototype.slice.call( array );

	if ( results ) {
		results.push.apply( results, array );
		return results;
	}
	
	return array;
};

// Perform a simple check to determine if the browser is capable of
// converting a NodeList to an array using builtin methods.
try {
	Array.prototype.slice.call( document.documentElement.childNodes );

// Provide a fallback method if it does not work
} catch(e){
	makeArray = function(array, results) {
		var ret = results || [];

		if ( toString.call(array) === "[object Array]" ) {
			Array.prototype.push.apply( ret, array );
		} else {
			if ( typeof array.length === "number" ) {
				for ( var i = 0, l = array.length; i < l; i++ ) {
					ret.push( array[i] );
				}
			} else {
				for ( var i = 0; array[i]; i++ ) {
					ret.push( array[i] );
				}
			}
		}

		return ret;
	};
}

var sortOrder;

if ( document.documentElement.compareDocumentPosition ) {
	sortOrder = function( a, b ) {
		var ret = a.compareDocumentPosition(b) & 4 ? -1 : a === b ? 0 : 1;
		if ( ret === 0 ) {
			hasDuplicate = true;
		}
		return ret;
	};
} else if ( "sourceIndex" in document.documentElement ) {
	sortOrder = function( a, b ) {
		var ret = a.sourceIndex - b.sourceIndex;
		if ( ret === 0 ) {
			hasDuplicate = true;
		}
		return ret;
	};
} else if ( document.createRange ) {
	sortOrder = function( a, b ) {
		var aRange = a.ownerDocument.createRange(), bRange = b.ownerDocument.createRange();
		aRange.selectNode(a);
		aRange.collapse(true);
		bRange.selectNode(b);
		bRange.collapse(true);
		var ret = aRange.compareBoundaryPoints(Range.START_TO_END, bRange);
		if ( ret === 0 ) {
			hasDuplicate = true;
		}
		return ret;
	};
}

// [vsdoc] The following function has been commented out for IntelliSense.
// Check to see if the browser returns elements by name when
// querying by getElementById (and provide a workaround)
//(function(){
//	// We're going to inject a fake input element with a specified name
//	var form = document.createElement("form"),
//		id = "script" + (new Date).getTime();
//	form.innerHTML = "<input name='" + id + "'/>";

//	// Inject it into the root element, check its status, and remove it quickly
//	var root = document.documentElement;
//	root.insertBefore( form, root.firstChild );

//	// The workaround has to do additional checks after a getElementById
//	// Which slows things down for other browsers (hence the branching)
//	if ( !!document.getElementById( id ) ) {
//		Expr.find.ID = function(match, context, isXML){
//			if ( typeof context.getElementById !== "undefined" && !isXML ) {
//				var m = context.getElementById(match[1]);
//				return m ? m.id === match[1] || typeof m.getAttributeNode !== "undefined" && m.getAttributeNode("id").nodeValue === match[1] ? [m] : undefined : [];
//			}
//		};

//		Expr.filter.ID = function(elem, match){
//			var node = typeof elem.getAttributeNode !== "undefined" && elem.getAttributeNode("id");
//			return elem.nodeType === 1 && node && node.nodeValue === match;
//		};
//	}

//	root.removeChild( form );
//})();

// [vsdoc] The following function has been commented out for IntelliSense.
//(function(){
//	// Check to see if the browser returns only elements
//	// when doing getElementsByTagName("*")

//	// Create a fake element
//	var div = document.createElement("div");
//	div.appendChild( document.createComment("") );

//	// Make sure no comments are found
//	if ( div.getElementsByTagName("*").length > 0 ) {
//		Expr.find.TAG = function(match, context){
//			var results = context.getElementsByTagName(match[1]);

//			// Filter out possible comments
//			if ( match[1] === "*" ) {
//				var tmp = [];

//				for ( var i = 0; results[i]; i++ ) {
//					if ( results[i].nodeType === 1 ) {
//						tmp.push( results[i] );
//					}
//				}

//				results = tmp;
//			}

//			return results;
//		};
//	}

//	// Check to see if an attribute returns normalized href attributes
//	div.innerHTML = "<a href='#'></a>";
//	if ( div.firstChild && typeof div.firstChild.getAttribute !== "undefined" &&
//			div.firstChild.getAttribute("href") !== "#" ) {
//		Expr.attrHandle.href = function(elem){
//			return elem.getAttribute("href", 2);
//		};
//	}
// })();

if ( document.querySelectorAll ) (function(){
	var oldSizzle = Sizzle, div = document.createElement("div");
	div.innerHTML = "<p class='TEST'></p>";

	// Safari can't handle uppercase or unicode characters when
	// in quirks mode.
	if ( div.querySelectorAll && div.querySelectorAll(".TEST").length === 0 ) {
		return;
	}
	
	Sizzle = function(query, context, extra, seed){
		context = context || document;

		// Only use querySelectorAll on non-XML documents
		// (ID selectors don't work in non-HTML documents)
		if ( !seed && context.nodeType === 9 && !isXML(context) ) {
			try {
				return makeArray( context.querySelectorAll(query), extra );
			} catch(e){}
		}
		
		return oldSizzle(query, context, extra, seed);
	};

	Sizzle.find = oldSizzle.find;
	Sizzle.filter = oldSizzle.filter;
	Sizzle.selectors = oldSizzle.selectors;
	Sizzle.matches = oldSizzle.matches;
})();

if ( document.getElementsByClassName && document.documentElement.getElementsByClassName ) (function(){
	var div = document.createElement("div");
	div.innerHTML = "<div class='test e'></div><div class='test'></div>";

	// Opera can't find a second classname (in 9.6)
	if ( div.getElementsByClassName("e").length === 0 )
		return;

	// Safari caches class attributes, doesn't catch changes (in 3.2)
	div.lastChild.className = "e";

	if ( div.getElementsByClassName("e").length === 1 )
		return;

	Expr.order.splice(1, 0, "CLASS");
	Expr.find.CLASS = function(match, context, isXML) {
		if ( typeof context.getElementsByClassName !== "undefined" && !isXML ) {
			return context.getElementsByClassName(match[1]);
		}
	};
})();

function dirNodeCheck( dir, cur, doneName, checkSet, nodeCheck, isXML ) {
	var sibDir = dir == "previousSibling" && !isXML;
	for ( var i = 0, l = checkSet.length; i < l; i++ ) {
		var elem = checkSet[i];
		if ( elem ) {
			if ( sibDir && elem.nodeType === 1 ){
				elem.sizcache = doneName;
				elem.sizset = i;
			}
			elem = elem[dir];
			var match = false;

			while ( elem ) {
				if ( elem.sizcache === doneName ) {
					match = checkSet[elem.sizset];
					break;
				}

				if ( elem.nodeType === 1 && !isXML ){
					elem.sizcache = doneName;
					elem.sizset = i;
				}

				if ( elem.nodeName === cur ) {
					match = elem;
					break;
				}

				elem = elem[dir];
			}

			checkSet[i] = match;
		}
	}
}

function dirCheck( dir, cur, doneName, checkSet, nodeCheck, isXML ) {
	var sibDir = dir == "previousSibling" && !isXML;
	for ( var i = 0, l = checkSet.length; i < l; i++ ) {
		var elem = checkSet[i];
		if ( elem ) {
			if ( sibDir && elem.nodeType === 1 ) {
				elem.sizcache = doneName;
				elem.sizset = i;
			}
			elem = elem[dir];
			var match = false;

			while ( elem ) {
				if ( elem.sizcache === doneName ) {
					match = checkSet[elem.sizset];
					break;
				}

				if ( elem.nodeType === 1 ) {
					if ( !isXML ) {
						elem.sizcache = doneName;
						elem.sizset = i;
					}
					if ( typeof cur !== "string" ) {
						if ( elem === cur ) {
							match = true;
							break;
						}

					} else if ( Sizzle.filter( cur, [elem] ).length > 0 ) {
						match = elem;
						break;
					}
				}

				elem = elem[dir];
			}

			checkSet[i] = match;
		}
	}
}

var contains = document.compareDocumentPosition ?  function(a, b){
	return a.compareDocumentPosition(b) & 16;
} : function(a, b){
	return a !== b && (a.contains ? a.contains(b) : true);
};

var isXML = function(elem){
	return elem.nodeType === 9 && elem.documentElement.nodeName !== "HTML" ||
		!!elem.ownerDocument && isXML( elem.ownerDocument );
};

var posProcess = function(selector, context){
	var tmpSet = [], later = "", match,
		root = context.nodeType ? [context] : context;

	// Position selectors must be done after the filter
	// And so must :not(positional) so we move all PSEUDOs to the end
	while ( (match = Expr.match.PSEUDO.exec( selector )) ) {
		later += match[0];
		selector = selector.replace( Expr.match.PSEUDO, "" );
	}

	selector = Expr.relative[selector] ? selector + "*" : selector;

	for ( var i = 0, l = root.length; i < l; i++ ) {
		Sizzle( selector, root[i], tmpSet );
	}

	return Sizzle.filter( later, tmpSet );
};

// EXPOSE
jQuery.find = Sizzle;
jQuery.filter = Sizzle.filter;
jQuery.expr = Sizzle.selectors;
jQuery.expr[":"] = jQuery.expr.filters;

Sizzle.selectors.filters.hidden = function(elem){
	return elem.offsetWidth === 0 || elem.offsetHeight === 0;
};

Sizzle.selectors.filters.visible = function(elem){
	return elem.offsetWidth > 0 || elem.offsetHeight > 0;
};

Sizzle.selectors.filters.animated = function(elem){
	return jQuery.grep(jQuery.timers, function(fn){
		return elem === fn.elem;
	}).length;
};

jQuery.multiFilter = function( expr, elems, not ) {
	///	<summary>
	///		This member is internal only.
	///	</summary>
	///	<private />

	if ( not ) {
		expr = ":not(" + expr + ")";
	}

	return Sizzle.matches(expr, elems);
};

jQuery.dir = function( elem, dir ){
	///	<summary>
	///		This member is internal only.
	///	</summary>
	///	<private />
	// This member is not documented in the jQuery API: http://docs.jquery.com/Special:Search?ns0=1&search=dir
	var matched = [], cur = elem[dir];
	while ( cur && cur != document ) {
		if ( cur.nodeType == 1 )
			matched.push( cur );
		cur = cur[dir];
	}
	return matched;
};

jQuery.nth = function(cur, result, dir, elem){
	///	<summary>
	///		This member is internal only.
	///	</summary>
	///	<private />
	// This member is not documented in the jQuery API: http://docs.jquery.com/Special:Search?ns0=1&search=nth
	result = result || 1;
	var num = 0;

	for ( ; cur; cur = cur[dir] )
		if ( cur.nodeType == 1 && ++num == result )
			break;

	return cur;
};

jQuery.sibling = function(n, elem){
	///	<summary>
	///		This member is internal only.
	///	</summary>
	///	<private />
	// This member is not documented in the jQuery API: http://docs.jquery.com/Special:Search?ns0=1&search=nth
	var r = [];

	for ( ; n; n = n.nextSibling ) {
		if ( n.nodeType == 1 && n != elem )
			r.push( n );
	}

	return r;
};

return;

window.Sizzle = Sizzle;

})();
/*
 * A number of helper functions used for managing events.
 * Many of the ideas behind this code originated from
 * Dean Edwards' addEvent library.
 */
jQuery.event = {

	// Bind an event to an element
	// Original by Dean Edwards
	add: function(elem, types, handler, data) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		if ( elem.nodeType == 3 || elem.nodeType == 8 )
			return;

		// For whatever reason, IE has trouble passing the window object
		// around, causing it to be cloned in the process
		if ( elem.setInterval && elem != window )
			elem = window;

		// Make sure that the function being executed has a unique ID
		if ( !handler.guid )
			handler.guid = this.guid++;

		// if data is passed, bind to handler
		if ( data !== undefined ) {
			// Create temporary function pointer to original handler
			var fn = handler;

			// Create unique handler function, wrapped around original handler
			handler = this.proxy( fn );

			// Store data in unique handler
			handler.data = data;
		}

		// Init the element's event structure
		var events = jQuery.data(elem, "events") || jQuery.data(elem, "events", {}),
			handle = jQuery.data(elem, "handle") || jQuery.data(elem, "handle", function(){
				// Handle the second event of a trigger and when
				// an event is called after a page has unloaded
				return typeof jQuery !== "undefined" && !jQuery.event.triggered ?
					jQuery.event.handle.apply(arguments.callee.elem, arguments) :
					undefined;
			});
		// Add elem as a property of the handle function
		// This is to prevent a memory leak with non-native
		// event in IE.
		handle.elem = elem;

		// Handle multiple events separated by a space
		// jQuery(...).bind("mouseover mouseout", fn);
		jQuery.each(types.split(/\s+/), function(index, type) {
			// Namespaced event handlers
			var namespaces = type.split(".");
			type = namespaces.shift();
			handler.type = namespaces.slice().sort().join(".");

			// Get the current list of functions bound to this event
			var handlers = events[type];
			
			if ( jQuery.event.specialAll[type] )
				jQuery.event.specialAll[type].setup.call(elem, data, namespaces);

			// Init the event handler queue
			if (!handlers) {
				handlers = events[type] = {};

				// Check for a special event handler
				// Only use addEventListener/attachEvent if the special
				// events handler returns false
				if ( !jQuery.event.special[type] || jQuery.event.special[type].setup.call(elem, data, namespaces) === false ) {
					// Bind the global event handler to the element
					if (elem.addEventListener)
						elem.addEventListener(type, handle, false);
					else if (elem.attachEvent)
						elem.attachEvent("on" + type, handle);
				}
			}

			// Add the function to the element's handler list
			handlers[handler.guid] = handler;

			// Keep track of which events have been used, for global triggering
			jQuery.event.global[type] = true;
		});

		// Nullify elem to prevent memory leaks in IE
		elem = null;
	},

	guid: 1,
	global: {},

	// Detach an event or set of events from an element
	remove: function(elem, types, handler) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		// don't do events on text and comment nodes
		if ( elem.nodeType == 3 || elem.nodeType == 8 )
			return;

		var events = jQuery.data(elem, "events"), ret, index;

		if ( events ) {
			// Unbind all events for the element
			if ( types === undefined || (typeof types === "string" && types.charAt(0) == ".") )
				for ( var type in events )
					this.remove( elem, type + (types || "") );
			else {
				// types is actually an event object here
				if ( types.type ) {
					handler = types.handler;
					types = types.type;
				}

				// Handle multiple events seperated by a space
				// jQuery(...).unbind("mouseover mouseout", fn);
				jQuery.each(types.split(/\s+/), function(index, type){
					// Namespaced event handlers
					var namespaces = type.split(".");
					type = namespaces.shift();
					var namespace = RegExp("(^|\\.)" + namespaces.slice().sort().join(".*\\.") + "(\\.|$)");

					if ( events[type] ) {
						// remove the given handler for the given type
						if ( handler )
							delete events[type][handler.guid];

						// remove all handlers for the given type
						else
							for ( var handle in events[type] )
								// Handle the removal of namespaced events
								if ( namespace.test(events[type][handle].type) )
									delete events[type][handle];
									
						if ( jQuery.event.specialAll[type] )
							jQuery.event.specialAll[type].teardown.call(elem, namespaces);

						// remove generic event handler if no more handlers exist
						for ( ret in events[type] ) break;
						if ( !ret ) {
							if ( !jQuery.event.special[type] || jQuery.event.special[type].teardown.call(elem, namespaces) === false ) {
								if (elem.removeEventListener)
									elem.removeEventListener(type, jQuery.data(elem, "handle"), false);
								else if (elem.detachEvent)
									elem.detachEvent("on" + type, jQuery.data(elem, "handle"));
							}
							ret = null;
							delete events[type];
						}
					}
				});
			}

			// Remove the expando if it's no longer used
			for ( ret in events ) break;
			if ( !ret ) {
				var handle = jQuery.data( elem, "handle" );
				if ( handle ) handle.elem = null;
				jQuery.removeData( elem, "events" );
				jQuery.removeData( elem, "handle" );
			}
		}
	},

	// bubbling is internal
	trigger: function( event, data, elem, bubbling ) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		// Event object or event type
		var type = event.type || event;

		if( !bubbling ){
			event = typeof event === "object" ?
				// jQuery.Event object
				event[expando] ? event :
				// Object literal
				jQuery.extend( jQuery.Event(type), event ) :
				// Just the event type (string)
				jQuery.Event(type);

			if ( type.indexOf("!") >= 0 ) {
				event.type = type = type.slice(0, -1);
				event.exclusive = true;
			}

			// Handle a global trigger
			if ( !elem ) {
				// Don't bubble custom events when global (to avoid too much overhead)
				event.stopPropagation();
				// Only trigger if we've ever bound an event for it
				if ( this.global[type] )
					jQuery.each( jQuery.cache, function(){
						if ( this.events && this.events[type] )
							jQuery.event.trigger( event, data, this.handle.elem );
					});
			}

			// Handle triggering a single element

			// don't do events on text and comment nodes
			if ( !elem || elem.nodeType == 3 || elem.nodeType == 8 )
				return undefined;
			
			// Clean up in case it is reused
			event.result = undefined;
			event.target = elem;
			
			// Clone the incoming data, if any
			data = jQuery.makeArray(data);
			data.unshift( event );
		}

		event.currentTarget = elem;

		// Trigger the event, it is assumed that "handle" is a function
		var handle = jQuery.data(elem, "handle");
		if ( handle )
			handle.apply( elem, data );

		// Handle triggering native .onfoo handlers (and on links since we don't call .click() for links)
		if ( (!elem[type] || (jQuery.nodeName(elem, 'a') && type == "click")) && elem["on"+type] && elem["on"+type].apply( elem, data ) === false )
			event.result = false;

		// Trigger the native events (except for clicks on links)
		if ( !bubbling && elem[type] && !event.isDefaultPrevented() && !(jQuery.nodeName(elem, 'a') && type == "click") ) {
			this.triggered = true;
			try {
				elem[ type ]();
			// prevent IE from throwing an error for some hidden elements
			} catch (e) {}
		}

		this.triggered = false;

		if ( !event.isPropagationStopped() ) {
			var parent = elem.parentNode || elem.ownerDocument;
			if ( parent )
				jQuery.event.trigger(event, data, parent, true);
		}
	},

	handle: function(event) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		// returned undefined or false
		var all, handlers;

		event = arguments[0] = jQuery.event.fix( event || window.event );
		event.currentTarget = this;

		// Namespaced event handlers
		var namespaces = event.type.split(".");
		event.type = namespaces.shift();

		// Cache this now, all = true means, any handler
		all = !namespaces.length && !event.exclusive;
		
		var namespace = RegExp("(^|\\.)" + namespaces.slice().sort().join(".*\\.") + "(\\.|$)");

		handlers = ( jQuery.data(this, "events") || {} )[event.type];

		for ( var j in handlers ) {
			var handler = handlers[j];

			// Filter the functions by class
			if ( all || namespace.test(handler.type) ) {
				// Pass in a reference to the handler function itself
				// So that we can later remove it
				event.handler = handler;
				event.data = handler.data;

				var ret = handler.apply(this, arguments);

				if( ret !== undefined ){
					event.result = ret;
					if ( ret === false ) {
						event.preventDefault();
						event.stopPropagation();
					}
				}

				if( event.isImmediatePropagationStopped() )
					break;

			}
		}
	},

	props: "altKey attrChange attrName bubbles button cancelable charCode clientX clientY ctrlKey currentTarget data detail eventPhase fromElement handler keyCode metaKey newValue originalTarget pageX pageY prevValue relatedNode relatedTarget screenX screenY shiftKey srcElement target toElement view wheelDelta which".split(" "),

	fix: function(event) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		if ( event[expando] )
			return event;

		// store a copy of the original event object
		// and "clone" to set read-only properties
		var originalEvent = event;
		event = jQuery.Event( originalEvent );

		for ( var i = this.props.length, prop; i; ){
			prop = this.props[ --i ];
			event[ prop ] = originalEvent[ prop ];
		}

		// Fix target property, if necessary
		if ( !event.target )
			event.target = event.srcElement || document; // Fixes #1925 where srcElement might not be defined either

		// check if target is a textnode (safari)
		if ( event.target.nodeType == 3 )
			event.target = event.target.parentNode;

		// Add relatedTarget, if necessary
		if ( !event.relatedTarget && event.fromElement )
			event.relatedTarget = event.fromElement == event.target ? event.toElement : event.fromElement;

		// Calculate pageX/Y if missing and clientX/Y available
		if ( event.pageX == null && event.clientX != null ) {
			var doc = document.documentElement, body = document.body;
			event.pageX = event.clientX + (doc && doc.scrollLeft || body && body.scrollLeft || 0) - (doc.clientLeft || 0);
			event.pageY = event.clientY + (doc && doc.scrollTop || body && body.scrollTop || 0) - (doc.clientTop || 0);
		}

		// Add which for key events
		if ( !event.which && ((event.charCode || event.charCode === 0) ? event.charCode : event.keyCode) )
			event.which = event.charCode || event.keyCode;

		// Add metaKey to non-Mac browsers (use ctrl for PC's and Meta for Macs)
		if ( !event.metaKey && event.ctrlKey )
			event.metaKey = event.ctrlKey;

		// Add which for click: 1 == left; 2 == middle; 3 == right
		// Note: button is not normalized, so don't use it
		if ( !event.which && event.button )
			event.which = (event.button & 1 ? 1 : ( event.button & 2 ? 3 : ( event.button & 4 ? 2 : 0 ) ));

		return event;
	},

	proxy: function( fn, proxy ){
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		proxy = proxy || function(){ return fn.apply(this, arguments); };
		// Set the guid of unique handler to the same of original handler, so it can be removed
		proxy.guid = fn.guid = fn.guid || proxy.guid || this.guid++;
		// So proxy can be declared as an argument
		return proxy;
	},

	special: {
		ready: {
			///	<summary>
			///		This method is internal.
			///	</summary>
			///	<private />
			
			// Make sure the ready event is setup
			setup: bindReady,
			teardown: function() {}
		}
	},
	
	specialAll: {
		live: {
			setup: function( selector, namespaces ){
				jQuery.event.add( this, namespaces[0], liveHandler );
			},
			teardown:  function( namespaces ){
				if ( namespaces.length ) {
					var remove = 0, name = RegExp("(^|\\.)" + namespaces[0] + "(\\.|$)");
					
					jQuery.each( (jQuery.data(this, "events").live || {}), function(){
						if ( name.test(this.type) )
							remove++;
					});
					
					if ( remove < 1 )
						jQuery.event.remove( this, namespaces[0], liveHandler );
				}
			}
		}
	}
};

jQuery.Event = function( src ){
	// Allow instantiation without the 'new' keyword
	if( !this.preventDefault )
		return new jQuery.Event(src);
	
	// Event object
	if( src && src.type ){
		this.originalEvent = src;
		this.type = src.type;
	// Event type
	}else
		this.type = src;

	// timeStamp is buggy for some events on Firefox(#3843)
	// So we won't rely on the native value
	this.timeStamp = now();
	
	// Mark it as fixed
	this[expando] = true;
};

function returnFalse(){
	return false;
}
function returnTrue(){
	return true;
}

// jQuery.Event is based on DOM3 Events as specified by the ECMAScript Language Binding
// http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
jQuery.Event.prototype = {
	preventDefault: function() {
		this.isDefaultPrevented = returnTrue;

		var e = this.originalEvent;
		if( !e )
			return;
		// if preventDefault exists run it on the original event
		if (e.preventDefault)
			e.preventDefault();
		// otherwise set the returnValue property of the original event to false (IE)
		e.returnValue = false;
	},
	stopPropagation: function() {
		this.isPropagationStopped = returnTrue;

		var e = this.originalEvent;
		if( !e )
			return;
		// if stopPropagation exists run it on the original event
		if (e.stopPropagation)
			e.stopPropagation();
		// otherwise set the cancelBubble property of the original event to true (IE)
		e.cancelBubble = true;
	},
	stopImmediatePropagation:function(){
		this.isImmediatePropagationStopped = returnTrue;
		this.stopPropagation();
	},
	isDefaultPrevented: returnFalse,
	isPropagationStopped: returnFalse,
	isImmediatePropagationStopped: returnFalse
};
// Checks if an event happened on an element within another element
// Used in jQuery.event.special.mouseenter and mouseleave handlers
var withinElement = function(event) {
	// Check if mouse(over|out) are still within the same parent element
	var parent = event.relatedTarget;
	// Traverse up the tree
	while ( parent && parent != this )
		try { parent = parent.parentNode; }
		catch(e) { parent = this; }
	
	if( parent != this ){
		// set the correct event type
		event.type = event.data;
		// handle event if we actually just moused on to a non sub-element
		jQuery.event.handle.apply( this, arguments );
	}
};
	
jQuery.each({ 
	mouseover: 'mouseenter', 
	mouseout: 'mouseleave'
}, function( orig, fix ){
	jQuery.event.special[ fix ] = {
		setup: function(){
			///	<summary>
			///		This method is internal.
			///	</summary>
			///	<private />
			
			jQuery.event.add( this, orig, withinElement, fix );
		},
		teardown: function(){
			///	<summary>
			///		This method is internal.
			///	</summary>
			///	<private />
			
			jQuery.event.remove( this, orig, withinElement );
		}
	};			   
});

jQuery.fn.extend({
	bind: function( type, data, fn ) {
		///	<summary>
		///		Binds a handler to one or more events for each matched element.  Can also bind custom events.
		///	</summary>
		///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
		///	<param name="data" optional="true" type="Object">Additional data passed to the event handler as event.data</param>
		///	<param name="fn" type="Function">A function to bind to the event on each of the set of matched elements.  function callback(eventObject) such that this corresponds to the dom element.</param>
		
		return type == "unload" ? this.one(type, data, fn) : this.each(function(){
			jQuery.event.add( this, type, fn || data, fn && data );
		});
	},

	one: function( type, data, fn ) {
		///	<summary>
		///		Binds a handler to one or more events to be executed exactly once for each matched element.
		///	</summary>
		///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
		///	<param name="data" optional="true" type="Object">Additional data passed to the event handler as event.data</param>
		///	<param name="fn" type="Function">A function to bind to the event on each of the set of matched elements.  function callback(eventObject) such that this corresponds to the dom element.</param>
		
		var one = jQuery.event.proxy( fn || data, function(event) {
			jQuery(this).unbind(event, one);
			return (fn || data).apply( this, arguments );
		});
		return this.each(function(){
			jQuery.event.add( this, type, one, fn && data);
		});
	},

	unbind: function( type, fn ) {
		///	<summary>
		///		Unbinds a handler from one or more events for each matched element.
		///	</summary>
		///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
		///	<param name="fn" type="Function">A function to bind to the event on each of the set of matched elements.  function callback(eventObject) such that this corresponds to the dom element.</param>
		
		return this.each(function(){
			jQuery.event.remove( this, type, fn );
		});
	},

	trigger: function( type, data ) {
		///	<summary>
		///		Triggers a type of event on every matched element.
		///	</summary>
		///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
		///	<param name="data" optional="true" type="Array">Additional data passed to the event handler as additional arguments.</param>
		///	<param name="fn" type="Function">This parameter is undocumented.</param>
		
		return this.each(function(){
			jQuery.event.trigger( type, data, this );
		});
	},

	triggerHandler: function( type, data ) {
		///	<summary>
		///		Triggers all bound event handlers on an element for a specific event type without executing the browser's default actions.
		///	</summary>
		///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
		///	<param name="data" optional="true" type="Array">Additional data passed to the event handler as additional arguments.</param>
		///	<param name="fn" type="Function">This parameter is undocumented.</param>
	
		if( this[0] ){
			var event = jQuery.Event(type);
			event.preventDefault();
			event.stopPropagation();
			jQuery.event.trigger( event, data, this[0] );
			return event.result;
		}		
	},

	toggle: function( fn ) {
		///	<summary>
		///		Toggles among two or more function calls every other click.
		///	</summary>
		///	<param name="fn" type="Function">The functions among which to toggle execution</param>
		
		// Save reference to arguments for access in closure
		var args = arguments, i = 1;

		// link all the functions, so any of them can unbind this click handler
		while( i < args.length )
			jQuery.event.proxy( fn, args[i++] );

		return this.click( jQuery.event.proxy( fn, function(event) {
			// Figure out which function to execute
			this.lastToggle = ( this.lastToggle || 0 ) % i;

			// Make sure that clicks stop
			event.preventDefault();

			// and execute the function
			return args[ this.lastToggle++ ].apply( this, arguments ) || false;
		}));
	},

	hover: function(fnOver, fnOut) {
		///	<summary>
		///		Simulates hovering (moving the mouse on or off of an object).
		///	</summary>
		///	<param name="fnOver" type="Function">The function to fire when the mouse is moved over a matched element.</param>
		///	<param name="fnOut" type="Function">The function to fire when the mouse is moved off of a matched element.</param>
		
		return this.mouseenter(fnOver).mouseleave(fnOut);
	},

	ready: function(fn) {
		///	<summary>
		///		Binds a function to be executed whenever the DOM is ready to be traversed and manipulated.
		///	</summary>
		///	<param name="fn" type="Function">The function to be executed when the DOM is ready.</param>

		// Attach the listeners
		bindReady();

		// If the DOM is already ready
		if ( jQuery.isReady )
			// Execute the function immediately
			fn.call( document, jQuery );

		// Otherwise, remember the function for later
		else
			// Add the function to the wait list
			jQuery.readyList.push( fn );

		return this;
	},
	
	live: function( type, fn ){
		///	<summary>
		///		Binds a handler to an event (like click) for all current - and future - matched element. Can also bind custom events.
		///	</summary>
		///	<param name="type" type="String">An event type</param>
		///	<param name="fn" type="Function">A function to bind to the event on each of the set of matched elements</param>

		var proxy = jQuery.event.proxy( fn );
		proxy.guid += this.selector + type;

		jQuery(document).bind( liveConvert(type, this.selector), this.selector, proxy );

		return this;
	},
	
	die: function( type, fn ){
		///	<summary>
		///		This does the opposite of live, it removes a bound live event.
		///		You can also unbind custom events registered with live.
		///		If the type is provided, all bound live events of that type are removed.
		///		If the function that was passed to live is provided as the second argument, only that specific event handler is removed.
		///	</summary>
		///	<param name="type" type="String">A live event type to unbind.</param>
		///	<param name="fn" type="Function">A function to unbind from the event on each of the set of matched elements.</param>

		jQuery(document).unbind( liveConvert(type, this.selector), fn ? { guid: fn.guid + this.selector + type } : null );
		return this;
	}
});

function liveHandler( event ){
	var check = RegExp("(^|\\.)" + event.type + "(\\.|$)"),
		stop = true,
		elems = [];

	jQuery.each(jQuery.data(this, "events").live || [], function(i, fn){
		if ( check.test(fn.type) ) {
			var elem = jQuery(event.target).closest(fn.data)[0];
			if ( elem )
				elems.push({ elem: elem, fn: fn });
		}
	});

	elems.sort(function(a,b) {
		return jQuery.data(a.elem, "closest") - jQuery.data(b.elem, "closest");
	});
	
	jQuery.each(elems, function(){
		if ( this.fn.call(this.elem, event, this.fn.data) === false )
			return (stop = false);
	});

	return stop;
}

function liveConvert(type, selector){
	return ["live", type, selector.replace(/\./g, "`").replace(/ /g, "|")].join(".");
}

jQuery.extend({
	isReady: false,
	readyList: [],
	// Handle when the DOM is ready
	ready: function() {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		// Make sure that the DOM is not already loaded
		if ( !jQuery.isReady ) {
			// Remember that the DOM is ready
			jQuery.isReady = true;

			// If there are functions bound, to execute
			if ( jQuery.readyList ) {
				// Execute all of them
				jQuery.each( jQuery.readyList, function(){
					this.call( document, jQuery );
				});

				// Reset the list of functions
				jQuery.readyList = null;
			}

			// Trigger any bound ready events
			jQuery(document).triggerHandler("ready");
		}
	}
});

var readyBound = false;

function bindReady(){
	if ( readyBound ) return;
	readyBound = true;

	// Mozilla, Opera and webkit nightlies currently support this event
	if ( document.addEventListener ) {
		// Use the handy event callback
		document.addEventListener( "DOMContentLoaded", function(){
			document.removeEventListener( "DOMContentLoaded", arguments.callee, false );
			jQuery.ready();
		}, false );

	// If IE event model is used
	} else if ( document.attachEvent ) {
		// ensure firing before onload,
		// maybe late but safe also for iframes
		document.attachEvent("onreadystatechange", function(){
			if ( document.readyState === "complete" ) {
				document.detachEvent( "onreadystatechange", arguments.callee );
				jQuery.ready();
			}
		});

		// If IE and not an iframe
		// continually check to see if the document is ready
		if ( document.documentElement.doScroll && window == window.top ) (function(){
			if ( jQuery.isReady ) return;

			try {
				// If IE is used, use the trick by Diego Perini
				// http://javascript.nwbox.com/IEContentLoaded/
				document.documentElement.doScroll("left");
			} catch( error ) {
				setTimeout( arguments.callee, 0 );
				return;
			}

			// and execute any waiting functions
			jQuery.ready();
		})();
	}

	// A fallback to window.onload, that will always work
	jQuery.event.add( window, "load", jQuery.ready );
}

// [vsdoc] The following section has been denormalized from original sources for IntelliSense.

jQuery.each( ("blur,focus,load,resize,scroll,unload,click,dblclick," +
	"mousedown,mouseup,mousemove,mouseover,mouseout,mouseenter,mouseleave," +
	"change,select,submit,keydown,keypress,keyup,error").split(","), function(i, name){

	// Handle event binding
	jQuery.fn[name] = function(fn){
		return fn ? this.bind(name, fn) : this.trigger(name);
	};
});

jQuery.fn["blur"] = function(fn) {
	///	<summary>
	///		1: blur() - Triggers the blur event of each matched element.
	///		2: blur(fn) - Binds a function to the blur event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("blur", fn) : this.trigger(name);
};

jQuery.fn["focus"] = function(fn) {
	///	<summary>
	///		1: focus() - Triggers the focus event of each matched element.
	///		2: focus(fn) - Binds a function to the focus event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("focus", fn) : this.trigger(name);
};

jQuery.fn["load"] = function(fn) {
	///	<summary>
	///		1: load() - Triggers the load event of each matched element.
	///		2: load(fn) - Binds a function to the load event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("load", fn) : this.trigger(name);
};

jQuery.fn["resize"] = function(fn) {
	///	<summary>
	///		1: resize() - Triggers the resize event of each matched element.
	///		2: resize(fn) - Binds a function to the resize event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("resize", fn) : this.trigger(name);
};

jQuery.fn["scroll"] = function(fn) {
	///	<summary>
	///		1: scroll() - Triggers the scroll event of each matched element.
	///		2: scroll(fn) - Binds a function to the scroll event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("scroll", fn) : this.trigger(name);
};

jQuery.fn["unload"] = function(fn) {
	///	<summary>
	///		1: unload() - Triggers the unload event of each matched element.
	///		2: unload(fn) - Binds a function to the unload event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("unload", fn) : this.trigger(name);
};

jQuery.fn["click"] = function(fn) {
	///	<summary>
	///		1: click() - Triggers the click event of each matched element.
	///		2: click(fn) - Binds a function to the click event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("click", fn) : this.trigger(name);
};

jQuery.fn["dblclick"] = function(fn) {
	///	<summary>
	///		1: dblclick() - Triggers the dblclick event of each matched element.
	///		2: dblclick(fn) - Binds a function to the dblclick event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("dblclick", fn) : this.trigger(name);
};

jQuery.fn["mousedown"] = function(fn) {
	///	<summary>
	///		Binds a function to the mousedown event of each matched element. 
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("mousedown", fn) : this.trigger(name);
};

jQuery.fn["mouseup"] = function(fn) {
	///	<summary>
	///		Bind a function to the mouseup event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("mouseup", fn) : this.trigger(name);
};

jQuery.fn["mousemove"] = function(fn) {
	///	<summary>
	///		Bind a function to the mousemove event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("mousemove", fn) : this.trigger(name);
};

jQuery.fn["mouseover"] = function(fn) {
	///	<summary>
	///		Bind a function to the mouseover event of each matched element. 
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("mouseover", fn) : this.trigger(name);
};

jQuery.fn["mouseout"] = function(fn) {
	///	<summary>
	///		Bind a function to the mouseout event of each matched element. 
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("mouseout", fn) : this.trigger(name);
};

jQuery.fn["mouseenter"] = function(fn) {
	///	<summary>
	///		Bind a function to the mouseenter event of each matched element. 
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("mouseenter", fn) : this.trigger(name);
};

jQuery.fn["mouseleave"] = function(fn) {
	///	<summary>
	///		Bind a function to the mouseleave event of each matched element. 
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("mouseleave", fn) : this.trigger(name);
};

jQuery.fn["change"] = function(fn) {
	///	<summary>
	///		1: change() - Triggers the change event of each matched element.
	///		2: change(fn) - Binds a function to the change event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("change", fn) : this.trigger(name);
};

jQuery.fn["select"] = function(fn) {
	///	<summary>
	///		1: select() - Triggers the select event of each matched element.
	///		2: select(fn) - Binds a function to the select event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("select", fn) : this.trigger(name);
};

jQuery.fn["submit"] = function(fn) {
	///	<summary>
	///		1: submit() - Triggers the submit event of each matched element.
	///		2: submit(fn) - Binds a function to the submit event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("submit", fn) : this.trigger(name);
};

jQuery.fn["keydown"] = function(fn) {
	///	<summary>
	///		1: keydown() - Triggers the keydown event of each matched element.
	///		2: keydown(fn) - Binds a function to the keydown event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("keydown", fn) : this.trigger(name);
};

jQuery.fn["keypress"] = function(fn) {
	///	<summary>
	///		1: keypress() - Triggers the keypress event of each matched element.
	///		2: keypress(fn) - Binds a function to the keypress event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("keypress", fn) : this.trigger(name);
};

jQuery.fn["keyup"] = function(fn) {
	///	<summary>
	///		1: keyup() - Triggers the keyup event of each matched element.
	///		2: keyup(fn) - Binds a function to the keyup event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("keyup", fn) : this.trigger(name);
};

jQuery.fn["error"] = function(fn) {
	///	<summary>
	///		1: error() - Triggers the error event of each matched element.
	///		2: error(fn) - Binds a function to the error event of each matched element.
	///	</summary>
	///	<param name="fn" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return fn ? this.bind("error", fn) : this.trigger(name);
};

// Prevent memory leaks in IE
// And prevent errors on refresh with events like mouseover in other browsers
// Window isn't included so as not to unbind existing unload events
jQuery( window ).bind( 'unload', function(){ 
	for ( var id in jQuery.cache )
		// Skip the window
		if ( id != 1 && jQuery.cache[ id ].handle )
			jQuery.event.remove( jQuery.cache[ id ].handle.elem );
});

// [vsdoc] The following function has been commented out for IntelliSense.
//(function(){

//	jQuery.support = {};

//	var root = document.documentElement,
//		script = document.createElement("script"),
//		div = document.createElement("div"),
//		id = "script" + (new Date).getTime();

//	div.style.display = "none";
//	
//	div.innerHTML = '   <link/><table></table><a href="/a" style="color:red;float:left;opacity:.5;">a</a><select><option>text</option></select><object><param/></object>';

//	var all = div.getElementsByTagName("*"),
//		a = div.getElementsByTagName("a")[0];

//	// Can't get basic test support
//	if ( !all || !all.length || !a ) {
//		return;
//	}

//	jQuery.support = {
//		// IE strips leading whitespace when .innerHTML is used
//		leadingWhitespace: div.firstChild.nodeType == 3,
//		
//		// Make sure that tbody elements aren't automatically inserted
//		// IE will insert them into empty tables
//		tbody: !div.getElementsByTagName("tbody").length,
//		
//		// Make sure that you can get all elements in an <object> element
//		// IE 7 always returns no results
//		objectAll: !!div.getElementsByTagName("object")[0]
//			.getElementsByTagName("*").length,
//		
//		// Make sure that link elements get serialized correctly by innerHTML
//		// This requires a wrapper element in IE
//		htmlSerialize: !!div.getElementsByTagName("link").length,
//		
//		// Get the style information from getAttribute
//		// (IE uses .cssText insted)
//		style: /red/.test( a.getAttribute("style") ),
//		
//		// Make sure that URLs aren't manipulated
//		// (IE normalizes it by default)
//		hrefNormalized: a.getAttribute("href") === "/a",
//		
//		// Make sure that element opacity exists
//		// (IE uses filter instead)
//		opacity: a.style.opacity === "0.5",
//		
//		// Verify style float existence
//		// (IE uses styleFloat instead of cssFloat)
//		cssFloat: !!a.style.cssFloat,

//		// Will be defined later
//		scriptEval: false,
//		noCloneEvent: true,
//		boxModel: null
//	};
//	
//	script.type = "text/javascript";
//	try {
//		script.appendChild( document.createTextNode( "window." + id + "=1;" ) );
//	} catch(e){}

//	root.insertBefore( script, root.firstChild );
//	
//	// Make sure that the execution of code works by injecting a script
//	// tag with appendChild/createTextNode
//	// (IE doesn't support this, fails, and uses .text instead)
//	if ( window[ id ] ) {
//		jQuery.support.scriptEval = true;
//		delete window[ id ];
//	}

//	root.removeChild( script );

//	if ( div.attachEvent && div.fireEvent ) {
//		div.attachEvent("onclick", function(){
//			// Cloning a node shouldn't copy over any
//			// bound event handlers (IE does this)
//			jQuery.support.noCloneEvent = false;
//			div.detachEvent("onclick", arguments.callee);
//		});
//		div.cloneNode(true).fireEvent("onclick");
//	}

//	// Figure out if the W3C box model works as expected
//	// document.body must exist before we can do this
//	jQuery(function(){
//		var div = document.createElement("div");
//		div.style.width = div.style.paddingLeft = "1px";

//		document.body.appendChild( div );
//		jQuery.boxModel = jQuery.support.boxModel = div.offsetWidth === 2;
//		document.body.removeChild( div ).style.display = 'none';
//	});
//})();

// [vsdoc] The following function has been modified for IntelliSense.
// var styleFloat = jQuery.support.cssFloat ? "cssFloat" : "styleFloat";
var styleFloat = "cssFloat";


jQuery.props = {
	"for": "htmlFor",
	"class": "className",
	"float": styleFloat,
	cssFloat: styleFloat,
	styleFloat: styleFloat,
	readonly: "readOnly",
	maxlength: "maxLength",
	cellspacing: "cellSpacing",
	rowspan: "rowSpan",
	tabindex: "tabIndex"
};
jQuery.fn.extend({
	// Keep a copy of the old load
	_load: jQuery.fn.load,

	load: function( url, params, callback ) {
		///	<summary>
		///		Loads HTML from a remote file and injects it into the DOM.  By default performs a GET request, but if parameters are included
		///		then a POST will be performed.
		///	</summary>
		///	<param name="url" type="String">The URL of the HTML page to load.</param>
		///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
		///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(responseText, textStatus, XMLHttpRequest) such that this maps the injected DOM element.</param>
		///	<returns type="jQuery" />
		
		if ( typeof url !== "string" )
			return this._load( url );

		var off = url.indexOf(" ");
		if ( off >= 0 ) {
			var selector = url.slice(off, url.length);
			url = url.slice(0, off);
		}

		// Default to a GET request
		var type = "GET";

		// If the second parameter was provided
		if ( params )
			// If it's a function
			if ( jQuery.isFunction( params ) ) {
				// We assume that it's the callback
				callback = params;
				params = null;

			// Otherwise, build a param string
			} else if( typeof params === "object" ) {
				params = jQuery.param( params );
				type = "POST";
			}

		var self = this;

		// Request the remote document
		jQuery.ajax({
			url: url,
			type: type,
			dataType: "html",
			data: params,
			complete: function(res, status){
				// If successful, inject the HTML into all the matched elements
				if ( status == "success" || status == "notmodified" )
					// See if a selector was specified
					self.html( selector ?
						// Create a dummy div to hold the results
						jQuery("<div/>")
							// inject the contents of the document in, removing the scripts
							// to avoid any 'Permission Denied' errors in IE
							.append(res.responseText.replace(/<script(.|\s)*?\/script>/g, ""))

							// Locate the specified elements
							.find(selector) :

						// If not, just inject the full result
						res.responseText );

				if( callback )
					self.each( callback, [res.responseText, status, res] );
			}
		});
		return this;
	},

	serialize: function() {
		///	<summary>
		///		Serializes a set of input elements into a string of data.
		///	</summary>
		///	<returns type="String">The serialized result</returns>
	
		return jQuery.param(this.serializeArray());
	},
	serializeArray: function() {
		///	<summary>
		///		Serializes all forms and form elements but returns a JSON data structure.
		///	</summary>
		///	<returns type="String">A JSON data structure representing the serialized items.</returns>
	
		return this.map(function(){
			return this.elements ? jQuery.makeArray(this.elements) : this;
		})
		.filter(function(){
			return this.name && !this.disabled &&
				(this.checked || /select|textarea/i.test(this.nodeName) ||
					/text|hidden|password|search/i.test(this.type));
		})
		.map(function(i, elem){
			var val = jQuery(this).val();
			return val == null ? null :
				jQuery.isArray(val) ?
					jQuery.map( val, function(val, i){
						return {name: elem.name, value: val};
					}) :
					{name: elem.name, value: val};
		}).get();
	}
});

// [vsdoc] The following section has been denormalized from original sources for IntelliSense.
// Attach a bunch of functions for handling common AJAX events
// jQuery.each( "ajaxStart,ajaxStop,ajaxComplete,ajaxError,ajaxSuccess,ajaxSend".split(","), function(i,o){
// 	jQuery.fn[o] = function(f){
// 		return this.bind(o, f);
// 	};
// });

jQuery.fn["ajaxStart"] = function(callback) {
	///	<summary>
	///		Attach a function to be executed whenever an AJAX request begins and there is none already active. This is an Ajax Event.
	///	</summary>
	///	<param name="callback" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return this.bind("ajaxStart", f);
};

jQuery.fn["ajaxStop"] = function(callback) {
	///	<summary>
	///		Attach a function to be executed whenever all AJAX requests have ended. This is an Ajax Event.
	///	</summary>
	///	<param name="callback" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return this.bind("ajaxStop", f);
};

jQuery.fn["ajaxComplete"] = function(callback) {
	///	<summary>
	///		Attach a function to be executed whenever an AJAX request completes. This is an Ajax Event.
	///	</summary>
	///	<param name="callback" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return this.bind("ajaxComplete", f);
};

jQuery.fn["ajaxError"] = function(callback) {
	///	<summary>
	///		Attach a function to be executed whenever an AJAX request fails. This is an Ajax Event.
	///	</summary>
	///	<param name="callback" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return this.bind("ajaxError", f);
};

jQuery.fn["ajaxSuccess"] = function(callback) {
	///	<summary>
	///		Attach a function to be executed whenever an AJAX request completes successfully. This is an Ajax Event.
	///	</summary>
	///	<param name="callback" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return this.bind("ajaxSuccess", f);
};

jQuery.fn["ajaxSend"] = function(callback) {
	///	<summary>
	///		Attach a function to be executed before an AJAX request is sent. This is an Ajax Event.
	///	</summary>
	///	<param name="callback" type="Function">The function to execute.</param>
	///	<returns type="jQuery" />
	return this.bind("ajaxSend", f);
};


var jsc = now();

jQuery.extend({
  
	get: function( url, data, callback, type ) {
		///	<summary>
		///		Loads a remote page using an HTTP GET request.
		///	</summary>
		///	<param name="url" type="String">The URL of the HTML page to load.</param>
		///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
		///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(responseText, textStatus) such that this maps the options for this AJAX request.</param>
		///	<param name="type" optional="true" type="String">Type of data to be returned to callback function.  Valid valiues are xml, html, script, json, text, _default.</param>
		///	<returns type="XMLHttpRequest" />
		
		// shift arguments if data argument was ommited
		if ( jQuery.isFunction( data ) ) {
			callback = data;
			data = null;
		}

		return jQuery.ajax({
			type: "GET",
			url: url,
			data: data,
			success: callback,
			dataType: type
		});
	},

	getScript: function( url, callback ) {
		///	<summary>
		///		Loads and executes a local JavaScript file using an HTTP GET request.
		///	</summary>
		///	<param name="url" type="String">The URL of the script to load.</param>
		///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(data, textStatus) such that this maps the options for the AJAX request.</param>
		///	<returns type="XMLHttpRequest" />
	
		return jQuery.get(url, null, callback, "script");
	},

	getJSON: function( url, data, callback ) {
		///	<summary>
		///		Loads JSON data using an HTTP GET request.
		///	</summary>
		///	<param name="url" type="String">The URL of the JSON data to load.</param>
		///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
		///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete if the data is loaded successfully.  It should map function(data, textStatus) such that this maps the options for this AJAX request.</param>
		///	<returns type="XMLHttpRequest" />
	
		return jQuery.get(url, data, callback, "json");
	},

	post: function( url, data, callback, type ) {
		///	<summary>
		///		Loads a remote page using an HTTP POST request.
		///	</summary>
		///	<param name="url" type="String">The URL of the HTML page to load.</param>
		///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
		///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(responseText, textStatus) such that this maps the options for this AJAX request.</param>
		///	<param name="type" optional="true" type="String">Type of data to be returned to callback function.  Valid valiues are xml, html, script, json, text, _default.</param>
		///	<returns type="XMLHttpRequest" />
		
		if ( jQuery.isFunction( data ) ) {
			callback = data;
			data = {};
		}

		return jQuery.ajax({
			type: "POST",
			url: url,
			data: data,
			success: callback,
			dataType: type
		});
	},

	ajaxSetup: function( settings ) {
		///	<summary>
		///		Sets up global settings for AJAX requests.
		///	</summary>
		///	<param name="settings" type="Options">A set of key/value pairs that configure the default Ajax request.</param>
	
		jQuery.extend( jQuery.ajaxSettings, settings );
	},

	ajaxSettings: {
		url: location.href,
		global: true,
		type: "GET",
		contentType: "application/x-www-form-urlencoded",
		processData: true,
		async: true,
		/*
		timeout: 0,
		data: null,
		username: null,
		password: null,
		*/
		// Create the request object; Microsoft failed to properly
		// implement the XMLHttpRequest in IE7, so we use the ActiveXObject when it is available
		// This function can be overriden by calling jQuery.ajaxSetup
		xhr:function(){
			return window.ActiveXObject ? new ActiveXObject("Microsoft.XMLHTTP") : new XMLHttpRequest();
		},
		accepts: {
			xml: "application/xml, text/xml",
			html: "text/html",
			script: "text/javascript, application/javascript",
			json: "application/json, text/javascript",
			text: "text/plain",
			_default: "*/*"
		}
	},

	// Last-Modified header cache for next request
	lastModified: {},

	ajax: function( s ) {
		///	<summary>
		///		Load a remote page using an HTTP request.
		///	</summary>
		///	<private />
		
		// Extend the settings, but re-extend 's' so that it can be
		// checked again later (in the test suite, specifically)
		s = jQuery.extend(true, s, jQuery.extend(true, {}, jQuery.ajaxSettings, s));

		var jsonp, jsre = /=\?(&|$)/g, status, data,
			type = s.type.toUpperCase();

		// convert data if not already a string
		if ( s.data && s.processData && typeof s.data !== "string" )
			s.data = jQuery.param(s.data);

		// Handle JSONP Parameter Callbacks
		if ( s.dataType == "jsonp" ) {
			if ( type == "GET" ) {
				if ( !s.url.match(jsre) )
					s.url += (s.url.match(/\?/) ? "&" : "?") + (s.jsonp || "callback") + "=?";
			} else if ( !s.data || !s.data.match(jsre) )
				s.data = (s.data ? s.data + "&" : "") + (s.jsonp || "callback") + "=?";
			s.dataType = "json";
		}

		// Build temporary JSONP function
		if ( s.dataType == "json" && (s.data && s.data.match(jsre) || s.url.match(jsre)) ) {
			jsonp = "jsonp" + jsc++;

			// Replace the =? sequence both in the query string and the data
			if ( s.data )
				s.data = (s.data + "").replace(jsre, "=" + jsonp + "$1");
			s.url = s.url.replace(jsre, "=" + jsonp + "$1");

			// We need to make sure
			// that a JSONP style response is executed properly
			s.dataType = "script";

			// Handle JSONP-style loading
			window[ jsonp ] = function(tmp){
				data = tmp;
				success();
				complete();
				// Garbage collect
				window[ jsonp ] = undefined;
				try{ delete window[ jsonp ]; } catch(e){}
				if ( head )
					head.removeChild( script );
			};
		}

		if ( s.dataType == "script" && s.cache == null )
			s.cache = false;

		if ( s.cache === false && type == "GET" ) {
			var ts = now();
			// try replacing _= if it is there
			var ret = s.url.replace(/(\?|&)_=.*?(&|$)/, "$1_=" + ts + "$2");
			// if nothing was replaced, add timestamp to the end
			s.url = ret + ((ret == s.url) ? (s.url.match(/\?/) ? "&" : "?") + "_=" + ts : "");
		}

		// If data is available, append data to url for get requests
		if ( s.data && type == "GET" ) {
			s.url += (s.url.match(/\?/) ? "&" : "?") + s.data;

			// IE likes to send both get and post data, prevent this
			s.data = null;
		}

		// Watch for a new set of requests
		if ( s.global && ! jQuery.active++ )
			jQuery.event.trigger( "ajaxStart" );

		// Matches an absolute URL, and saves the domain
		var parts = /^(\w+:)?\/\/([^\/?#]+)/.exec( s.url );

		// If we're requesting a remote document
		// and trying to load JSON or Script with a GET
		if ( s.dataType == "script" && type == "GET" && parts
			&& ( parts[1] && parts[1] != location.protocol || parts[2] != location.host )){

			var head = document.getElementsByTagName("head")[0];
			var script = document.createElement("script");
			script.src = s.url;
			if (s.scriptCharset)
				script.charset = s.scriptCharset;

			// Handle Script loading
			if ( !jsonp ) {
				var done = false;

				// Attach handlers for all browsers
				script.onload = script.onreadystatechange = function(){
					if ( !done && (!this.readyState ||
							this.readyState == "loaded" || this.readyState == "complete") ) {
						done = true;
						success();
						complete();

						// Handle memory leak in IE
						script.onload = script.onreadystatechange = null;
						head.removeChild( script );
					}
				};
			}

			head.appendChild(script);

			// We handle everything using the script element injection
			return undefined;
		}

		var requestDone = false;

		// Create the request object
		var xhr = s.xhr();

		// Open the socket
		// Passing null username, generates a login popup on Opera (#2865)
		if( s.username )
			xhr.open(type, s.url, s.async, s.username, s.password);
		else
			xhr.open(type, s.url, s.async);

		// Need an extra try/catch for cross domain requests in Firefox 3
		try {
			// Set the correct header, if data is being sent
			if ( s.data )
				xhr.setRequestHeader("Content-Type", s.contentType);

			// Set the If-Modified-Since header, if ifModified mode.
			if ( s.ifModified )
				xhr.setRequestHeader("If-Modified-Since",
					jQuery.lastModified[s.url] || "Thu, 01 Jan 1970 00:00:00 GMT" );

			// Set header so the called script knows that it's an XMLHttpRequest
			xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");

			// Set the Accepts header for the server, depending on the dataType
			xhr.setRequestHeader("Accept", s.dataType && s.accepts[ s.dataType ] ?
				s.accepts[ s.dataType ] + ", */*" :
				s.accepts._default );
		} catch(e){}

		// Allow custom headers/mimetypes and early abort
		if ( s.beforeSend && s.beforeSend(xhr, s) === false ) {
			// Handle the global AJAX counter
			if ( s.global && ! --jQuery.active )
				jQuery.event.trigger( "ajaxStop" );
			// close opended socket
			xhr.abort();
			return false;
		}

		if ( s.global )
			jQuery.event.trigger("ajaxSend", [xhr, s]);

		// Wait for a response to come back
		var onreadystatechange = function(isTimeout){
			// The request was aborted, clear the interval and decrement jQuery.active
			if (xhr.readyState == 0) {
				if (ival) {
					// clear poll interval
					clearInterval(ival);
					ival = null;
					// Handle the global AJAX counter
					if ( s.global && ! --jQuery.active )
						jQuery.event.trigger( "ajaxStop" );
				}
			// The transfer is complete and the data is available, or the request timed out
			} else if ( !requestDone && xhr && (xhr.readyState == 4 || isTimeout == "timeout") ) {
				requestDone = true;

				// clear poll interval
				if (ival) {
					clearInterval(ival);
					ival = null;
				}

				status = isTimeout == "timeout" ? "timeout" :
					!jQuery.httpSuccess( xhr ) ? "error" :
					s.ifModified && jQuery.httpNotModified( xhr, s.url ) ? "notmodified" :
					"success";

				if ( status == "success" ) {
					// Watch for, and catch, XML document parse errors
					try {
						// process the data (runs the xml through httpData regardless of callback)
						data = jQuery.httpData( xhr, s.dataType, s );
					} catch(e) {
						status = "parsererror";
					}
				}

				// Make sure that the request was successful or notmodified
				if ( status == "success" ) {
					// Cache Last-Modified header, if ifModified mode.
					var modRes;
					try {
						modRes = xhr.getResponseHeader("Last-Modified");
					} catch(e) {} // swallow exception thrown by FF if header is not available

					if ( s.ifModified && modRes )
						jQuery.lastModified[s.url] = modRes;

					// JSONP handles its own success callback
					if ( !jsonp )
						success();
				} else
					jQuery.handleError(s, xhr, status);

				// Fire the complete handlers
				complete();

				if ( isTimeout )
					xhr.abort();

				// Stop memory leaks
				if ( s.async )
					xhr = null;
			}
		};

		if ( s.async ) {
			// don't attach the handler to the request, just poll it instead
			var ival = setInterval(onreadystatechange, 13);

			// Timeout checker
			if ( s.timeout > 0 )
				setTimeout(function(){
					// Check to see if the request is still happening
					if ( xhr && !requestDone )
						onreadystatechange( "timeout" );
				}, s.timeout);
		}

		// Send the data
		try {
			xhr.send(s.data);
		} catch(e) {
			jQuery.handleError(s, xhr, null, e);
		}

		// firefox 1.5 doesn't fire statechange for sync requests
		if ( !s.async )
			onreadystatechange();

		function success(){
			// If a local callback was specified, fire it and pass it the data
			if ( s.success )
				s.success( data, status );

			// Fire the global callback
			if ( s.global )
				jQuery.event.trigger( "ajaxSuccess", [xhr, s] );
		}

		function complete(){
			// Process result
			if ( s.complete )
				s.complete(xhr, status);

			// The request was completed
			if ( s.global )
				jQuery.event.trigger( "ajaxComplete", [xhr, s] );

			// Handle the global AJAX counter
			if ( s.global && ! --jQuery.active )
				jQuery.event.trigger( "ajaxStop" );
		}

		// return XMLHttpRequest to allow aborting the request etc.
		return xhr;
	},

	handleError: function( s, xhr, status, e ) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />

		// If a local callback was specified, fire it
		if ( s.error ) s.error( xhr, status, e );

		// Fire the global callback
		if ( s.global )
			jQuery.event.trigger( "ajaxError", [xhr, s, e] );
	},

	// Counter for holding the number of active queries
	active: 0,

	// Determines if an XMLHttpRequest was successful or not
	httpSuccess: function( xhr ) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		try {
			// IE error sometimes returns 1223 when it should be 204 so treat it as success, see #1450
			return !xhr.status && location.protocol == "file:" ||
				( xhr.status >= 200 && xhr.status < 300 ) || xhr.status == 304 || xhr.status == 1223;
		} catch(e){}
		return false;
	},

	// Determines if an XMLHttpRequest returns NotModified
	httpNotModified: function( xhr, url ) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		try {
			var xhrRes = xhr.getResponseHeader("Last-Modified");

			// Firefox always returns 200. check Last-Modified date
			return xhr.status == 304 || xhrRes == jQuery.lastModified[url];
		} catch(e){}
		return false;
	},

	httpData: function( xhr, type, s ) {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		
		var ct = xhr.getResponseHeader("content-type"),
			xml = type == "xml" || !type && ct && ct.indexOf("xml") >= 0,
			data = xml ? xhr.responseXML : xhr.responseText;

		if ( xml && data.documentElement.tagName == "parsererror" )
			throw "parsererror";
			
		// Allow a pre-filtering function to sanitize the response
		// s != null is checked to keep backwards compatibility
		if( s && s.dataFilter )
			data = s.dataFilter( data, type );

		// The filter can actually parse the response
		if( typeof data === "string" ){

			// If the type is "script", eval it in global context
			if ( type == "script" )
				jQuery.globalEval( data );

			// Get the JavaScript object, if JSON is used.
			if ( type == "json" )
				data = window["eval"]("(" + data + ")");
		}
		
		return data;
	},

	// Serialize an array of form elements or a set of
	// key/values into a query string
	param: function( a ) {
		///	<summary>
		///		This method is internal.  Use serialize() instead.
		///	</summary>
		///	<param name="a" type="Map">A map of key/value pairs to serialize into a string.</param>'
		///	<returns type="String" />
		///	<private />
	
		var s = [ ];

		function add( key, value ){
			s[ s.length ] = encodeURIComponent(key) + '=' + encodeURIComponent(value);
		};

		// If an array was passed in, assume that it is an array
		// of form elements
		if ( jQuery.isArray(a) || a.jquery )
			// Serialize the form elements
			jQuery.each( a, function(){
				add( this.name, this.value );
			});

		// Otherwise, assume that it's an object of key/value pairs
		else
			// Serialize the key/values
			for ( var j in a )
				// If the value is an array then the key names need to be repeated
				if ( jQuery.isArray(a[j]) )
					jQuery.each( a[j], function(){
						add( j, this );
					});
				else
					add( j, jQuery.isFunction(a[j]) ? a[j]() : a[j] );

		// Return the resulting serialization
		return s.join("&").replace(/%20/g, "+");
	}

});
var elemdisplay = {},
	timerId,
	fxAttrs = [
		// height animations
		[ "height", "marginTop", "marginBottom", "paddingTop", "paddingBottom" ],
		// width animations
		[ "width", "marginLeft", "marginRight", "paddingLeft", "paddingRight" ],
		// opacity animations
		[ "opacity" ]
	];

function genFx( type, num ){
	var obj = {};
	jQuery.each( fxAttrs.concat.apply([], fxAttrs.slice(0,num)), function(){
		obj[ this ] = type;
	});
	return obj;
}

jQuery.fn.extend({
	show: function(speed,callback){
		///	<summary>
		///		Show all matched elements using a graceful animation and firing an optional callback after completion.
		///	</summary>
		///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
		///		the number of milliseconds to run the animation</param>
		///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
		///	<returns type="jQuery" />
		
		if ( speed ) {
			return this.animate( genFx("show", 3), speed, callback);
		} else {
			for ( var i = 0, l = this.length; i < l; i++ ){
				var old = jQuery.data(this[i], "olddisplay");
				
				this[i].style.display = old || "";
				
				if ( jQuery.css(this[i], "display") === "none" ) {
					var tagName = this[i].tagName, display;
					
					if ( elemdisplay[ tagName ] ) {
						display = elemdisplay[ tagName ];
					} else {
						var elem = jQuery("<" + tagName + " />").appendTo("body");
						
						display = elem.css("display");
						if ( display === "none" )
							display = "block";
						
						elem.remove();
						
						elemdisplay[ tagName ] = display;
					}
					
					jQuery.data(this[i], "olddisplay", display);
				}
			}

			// Set the display of the elements in a second loop
			// to avoid the constant reflow
			for ( var i = 0, l = this.length; i < l; i++ ){
				this[i].style.display = jQuery.data(this[i], "olddisplay") || "";
			}
			
			return this;
		}
	},

	hide: function(speed,callback){
		///	<summary>
		///		Hides all matched elements using a graceful animation and firing an optional callback after completion.
		///	</summary>
		///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
		///		the number of milliseconds to run the animation</param>
		///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
		///	<returns type="jQuery" />
	
		if ( speed ) {
			return this.animate( genFx("hide", 3), speed, callback);
		} else {
			for ( var i = 0, l = this.length; i < l; i++ ){
				var old = jQuery.data(this[i], "olddisplay");
				if ( !old && old !== "none" )
					jQuery.data(this[i], "olddisplay", jQuery.css(this[i], "display"));
			}

			// Set the display of the elements in a second loop
			// to avoid the constant reflow
			for ( var i = 0, l = this.length; i < l; i++ ){
				this[i].style.display = "none";
			}

			return this;
		}
	},

	// Save the old toggle function
	_toggle: jQuery.fn.toggle,

	toggle: function( fn, fn2 ){
		///	<summary>
		///		Toggles displaying each of the set of matched elements.
		///	</summary>
		///	<returns type="jQuery" />
		
		var bool = typeof fn === "boolean";

		return jQuery.isFunction(fn) && jQuery.isFunction(fn2) ?
			this._toggle.apply( this, arguments ) :
			fn == null || bool ?
				this.each(function(){
					var state = bool ? fn : jQuery(this).is(":hidden");
					jQuery(this)[ state ? "show" : "hide" ]();
				}) :
				this.animate(genFx("toggle", 3), fn, fn2);
	},

	fadeTo: function(speed,to,callback){
		///	<summary>
		///		Fades the opacity of all matched elements to a specified opacity.
		///	</summary>
		///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
		///		the number of milliseconds to run the animation</param>
		///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
		///	<returns type="jQuery" />
		return this.animate({opacity: to}, speed, callback);
	},

	animate: function( prop, speed, easing, callback ) {
		///	<summary>
		///		A function for making custom animations.
		///	</summary>
		///	<param name="prop" type="Options">A set of style attributes that you wish to animate and to what end.</param>
		///	<param name="speed" optional="true" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
		///		the number of milliseconds to run the animation</param>
		///	<param name="easing" optional="true" type="String">The name of the easing effect that you want to use.  There are two built-in values, 'linear' and 'swing'.</param>
		///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
		///	<returns type="jQuery" />

		var optall = jQuery.speed(speed, easing, callback);

		return this[ optall.queue === false ? "each" : "queue" ](function(){
		
			var opt = jQuery.extend({}, optall), p,
				hidden = this.nodeType == 1 && jQuery(this).is(":hidden"),
				self = this;
	
			for ( p in prop ) {
				if ( prop[p] == "hide" && hidden || prop[p] == "show" && !hidden )
					return opt.complete.call(this);

				if ( ( p == "height" || p == "width" ) && this.style ) {
					// Store display property
					opt.display = jQuery.css(this, "display");

					// Make sure that nothing sneaks out
					opt.overflow = this.style.overflow;
				}
			}

			if ( opt.overflow != null )
				this.style.overflow = "hidden";

			opt.curAnim = jQuery.extend({}, prop);

			jQuery.each( prop, function(name, val){
				var e = new jQuery.fx( self, opt, name );

				if ( /toggle|show|hide/.test(val) )
					e[ val == "toggle" ? hidden ? "show" : "hide" : val ]( prop );
				else {
					var parts = val.toString().match(/^([+-]=)?([\d+-.]+)(.*)$/),
						start = e.cur(true) || 0;

					if ( parts ) {
						var end = parseFloat(parts[2]),
							unit = parts[3] || "px";

						// We need to compute starting value
						if ( unit != "px" ) {
							self.style[ name ] = (end || 1) + unit;
							start = ((end || 1) / e.cur(true)) * start;
							self.style[ name ] = start + unit;
						}

						// If a +=/-= token was provided, we're doing a relative animation
						if ( parts[1] )
							end = ((parts[1] == "-=" ? -1 : 1) * end) + start;

						e.custom( start, end, unit );
					} else
						e.custom( start, val, "" );
				}
			});

			// For JS strict compliance
			return true;
		});
	},

	stop: function(clearQueue, gotoEnd){
		///	<summary>
		///		Stops all currently animations on the specified elements.
		///	</summary>
		///	<param name="clearQueue" optional="true" type="Boolean">True to clear animations that are queued to run.</param>
		///	<param name="gotoEnd" optional="true" type="Boolean">True to move the element value to the end of its animation target.</param>
		///	<returns type="jQuery" />
		
		var timers = jQuery.timers;

		if (clearQueue)
			this.queue([]);

		this.each(function(){
			// go in reverse order so anything added to the queue during the loop is ignored
			for ( var i = timers.length - 1; i >= 0; i-- )
				if ( timers[i].elem == this ) {
					if (gotoEnd)
						// force the next step to be the last
						timers[i](true);
					timers.splice(i, 1);
				}
		});

		// start the next in the queue if the last step wasn't forced
		if (!gotoEnd)
			this.dequeue();

		return this;
	}

});

// Generate shortcuts for custom animations
// jQuery.each({
// 	slideDown: genFx("show", 1),
// 	slideUp: genFx("hide", 1),
// 	slideToggle: genFx("toggle", 1),
// 	fadeIn: { opacity: "show" },
// 	fadeOut: { opacity: "hide" }
// }, function( name, props ){
// 	jQuery.fn[ name ] = function( speed, callback ){
// 		return this.animate( props, speed, callback );
// 	};
// });

// [vsdoc] The following section has been denormalized from original sources for IntelliSense.

jQuery.fn.slideDown = function( speed, callback ){
	///	<summary>
	///		Reveal all matched elements by adjusting their height.
	///	</summary>
	///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
	///		the number of milliseconds to run the animation</param>
	///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
	///	<returns type="jQuery" />
	return this.animate( genFx("show", 1), speed, callback );
};

jQuery.fn.slideUp = function( speed, callback ){
	///	<summary>
	///		Hiding all matched elements by adjusting their height.
	///	</summary>
	///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
	///		the number of milliseconds to run the animation</param>
	///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
	///	<returns type="jQuery" />
	return this.animate( genFx("hide", 1), speed, callback );
};

jQuery.fn.slideToggle = function( speed, callback ){
	///	<summary>
	///		Toggles the visibility of all matched elements by adjusting their height.
	///	</summary>
	///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
	///		the number of milliseconds to run the animation</param>
	///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
	///	<returns type="jQuery" />
	return this.animate( genFx("toggle", 1), speed, callback );
};

jQuery.fn.fadeIn = function( speed, callback ){
	///	<summary>
	///		Fades in all matched elements by adjusting their opacity.
	///	</summary>
	///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
	///		the number of milliseconds to run the animation</param>
	///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
	///	<returns type="jQuery" />
	return this.animate( { opacity: "show" }, speed, callback );
};

jQuery.fn.fadeOut = function( speed, callback ){
	///	<summary>
	///		Fades the opacity of all matched elements to a specified opacity.
	///	</summary>
	///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
	///		the number of milliseconds to run the animation</param>
	///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
	///	<returns type="jQuery" />
	return this.animate( { opacity: "hide" }, speed, callback );
};

jQuery.extend({

	speed: function(speed, easing, fn) {
		///	<summary>
		///		This member is internal.
		///	</summary>
		///	<private />
		var opt = typeof speed === "object" ? speed : {
			complete: fn || !fn && easing ||
				jQuery.isFunction( speed ) && speed,
			duration: speed,
			easing: fn && easing || easing && !jQuery.isFunction(easing) && easing
		};

		opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
			jQuery.fx.speeds[opt.duration] || jQuery.fx.speeds._default;

		// Queueing
		opt.old = opt.complete;
		opt.complete = function(){
			if ( opt.queue !== false )
				jQuery(this).dequeue();
			if ( jQuery.isFunction( opt.old ) )
				opt.old.call( this );
		};

		return opt;
	},

	easing: {
		linear: function( p, n, firstNum, diff ) {
   		///	<summary>
   		///		This member is internal.
			///	</summary>
   		///	<private />
			return firstNum + diff * p;
		},
		swing: function( p, n, firstNum, diff ) {
   		///	<summary>
   		///		This member is internal.
			///	</summary>
   		///	<private />
			return ((-Math.cos(p*Math.PI)/2) + 0.5) * diff + firstNum;
		}
	},

	timers: [],

	fx: function( elem, options, prop ){
		///	<summary>
		///		This member is internal.
		///	</summary>
		///	<private />
		this.options = options;
		this.elem = elem;
		this.prop = prop;

		if ( !options.orig )
			options.orig = {};
	}

});

jQuery.fx.prototype = {

	// Simple function for setting a style value
	update: function(){
		///	<summary>
		///		This member is internal.
		///	</summary>
		///	<private />
		if ( this.options.step )
			this.options.step.call( this.elem, this.now, this );

		(jQuery.fx.step[this.prop] || jQuery.fx.step._default)( this );

		// Set display property to block for height/width animations
		if ( ( this.prop == "height" || this.prop == "width" ) && this.elem.style )
			this.elem.style.display = "block";
	},

	// Get the current size
	cur: function(force){
		///	<summary>
		///		This member is internal.
		///	</summary>
		///	<private />
		if ( this.elem[this.prop] != null && (!this.elem.style || this.elem.style[this.prop] == null) )
			return this.elem[ this.prop ];

		var r = parseFloat(jQuery.css(this.elem, this.prop, force));
		return r && r > -10000 ? r : parseFloat(jQuery.curCSS(this.elem, this.prop)) || 0;
	},

	// Start an animation from one number to another
	custom: function(from, to, unit){
		this.startTime = now();
		this.start = from;
		this.end = to;
		this.unit = unit || this.unit || "px";
		this.now = this.start;
		this.pos = this.state = 0;

		var self = this;
		function t(gotoEnd){
			return self.step(gotoEnd);
		}

		t.elem = this.elem;

		if ( t() && jQuery.timers.push(t) && !timerId ) {
			timerId = setInterval(function(){
				var timers = jQuery.timers;

				for ( var i = 0; i < timers.length; i++ )
					if ( !timers[i]() )
						timers.splice(i--, 1);

				if ( !timers.length ) {
					clearInterval( timerId );
					timerId = undefined;
				}
			}, 13);
		}
	},

	// Simple 'show' function
	show: function(){
		///	<summary>
		///		Displays each of the set of matched elements if they are hidden.
		///	</summary>
		// Remember where we started, so that we can go back to it later
		this.options.orig[this.prop] = jQuery.attr( this.elem.style, this.prop );
		this.options.show = true;

		// Begin the animation
		// Make sure that we start at a small width/height to avoid any
		// flash of content
		this.custom(this.prop == "width" || this.prop == "height" ? 1 : 0, this.cur());

		// Start by showing the element
		jQuery(this.elem).show();
	},

	// Simple 'hide' function
	hide: function(){
		///	<summary>
		///		Hides each of the set of matched elements if they are shown.
		///	</summary>

		// Remember where we started, so that we can go back to it later
		this.options.orig[this.prop] = jQuery.attr( this.elem.style, this.prop );
		this.options.hide = true;

		// Begin the animation
		this.custom(this.cur(), 0);
	},

	// Each step of an animation
	step: function(gotoEnd){
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		var t = now();

		if ( gotoEnd || t >= this.options.duration + this.startTime ) {
			this.now = this.end;
			this.pos = this.state = 1;
			this.update();

			this.options.curAnim[ this.prop ] = true;

			var done = true;
			for ( var i in this.options.curAnim )
				if ( this.options.curAnim[i] !== true )
					done = false;

			if ( done ) {
				if ( this.options.display != null ) {
					// Reset the overflow
					this.elem.style.overflow = this.options.overflow;

					// Reset the display
					this.elem.style.display = this.options.display;
					if ( jQuery.css(this.elem, "display") == "none" )
						this.elem.style.display = "block";
				}

				// Hide the element if the "hide" operation was done
				if ( this.options.hide )
					jQuery(this.elem).hide();

				// Reset the properties, if the item has been hidden or shown
				if ( this.options.hide || this.options.show )
					for ( var p in this.options.curAnim )
						jQuery.attr(this.elem.style, p, this.options.orig[p]);
					
				// Execute the complete function
				this.options.complete.call( this.elem );
			}

			return false;
		} else {
			var n = t - this.startTime;
			this.state = n / this.options.duration;

			// Perform the easing function, defaults to swing
			this.pos = jQuery.easing[this.options.easing || (jQuery.easing.swing ? "swing" : "linear")](this.state, n, 0, 1, this.options.duration);
			this.now = this.start + ((this.end - this.start) * this.pos);

			// Perform the next step of the animation
			this.update();
		}

		return true;
	}

};

jQuery.extend( jQuery.fx, {
	speeds:{
		slow: 600,
 		fast: 200,
 		// Default speed
 		_default: 400
	},
	step: {

		opacity: function(fx){
			jQuery.attr(fx.elem.style, "opacity", fx.now);
		},

		_default: function(fx){
			if ( fx.elem.style && fx.elem.style[ fx.prop ] != null )
				fx.elem.style[ fx.prop ] = fx.now + fx.unit;
			else
				fx.elem[ fx.prop ] = fx.now;
		}
	}
});
if ( document.documentElement["getBoundingClientRect"] )
	jQuery.fn.offset = function() {
		///	<summary>
		///		Gets the current offset of the first matched element relative to the viewport.
		///	</summary>
		///	<returns type="Object">An object with two Integer properties, 'top' and 'left'.</returns>
		if ( !this[0] ) return { top: 0, left: 0 };
		if ( this[0] === this[0].ownerDocument.body ) return jQuery.offset.bodyOffset( this[0] );
		var box  = this[0].getBoundingClientRect(), doc = this[0].ownerDocument, body = doc.body, docElem = doc.documentElement,
			clientTop = docElem.clientTop || body.clientTop || 0, clientLeft = docElem.clientLeft || body.clientLeft || 0,
			top  = box.top  + (self.pageYOffset || jQuery.boxModel && docElem.scrollTop  || body.scrollTop ) - clientTop,
			left = box.left + (self.pageXOffset || jQuery.boxModel && docElem.scrollLeft || body.scrollLeft) - clientLeft;
		return { top: top, left: left };
	};
else 
	jQuery.fn.offset = function() {
		///	<summary>
		///		Gets the current offset of the first matched element relative to the viewport.
		///	</summary>
		///	<returns type="Object">An object with two Integer properties, 'top' and 'left'.</returns>
		if ( !this[0] ) return { top: 0, left: 0 };
		if ( this[0] === this[0].ownerDocument.body ) return jQuery.offset.bodyOffset( this[0] );
		jQuery.offset.initialized || jQuery.offset.initialize();

		var elem = this[0], offsetParent = elem.offsetParent, prevOffsetParent = elem,
			doc = elem.ownerDocument, computedStyle, docElem = doc.documentElement,
			body = doc.body, defaultView = doc.defaultView,
			prevComputedStyle = defaultView.getComputedStyle(elem, null),
			top = elem.offsetTop, left = elem.offsetLeft;

		while ( (elem = elem.parentNode) && elem !== body && elem !== docElem ) {
			computedStyle = defaultView.getComputedStyle(elem, null);
			top -= elem.scrollTop, left -= elem.scrollLeft;
			if ( elem === offsetParent ) {
				top += elem.offsetTop, left += elem.offsetLeft;
				if ( jQuery.offset.doesNotAddBorder && !(jQuery.offset.doesAddBorderForTableAndCells && /^t(able|d|h)$/i.test(elem.tagName)) )
					top  += parseInt( computedStyle.borderTopWidth,  10) || 0,
					left += parseInt( computedStyle.borderLeftWidth, 10) || 0;
				prevOffsetParent = offsetParent, offsetParent = elem.offsetParent;
			}
			if ( jQuery.offset.subtractsBorderForOverflowNotVisible && computedStyle.overflow !== "visible" )
				top  += parseInt( computedStyle.borderTopWidth,  10) || 0,
				left += parseInt( computedStyle.borderLeftWidth, 10) || 0;
			prevComputedStyle = computedStyle;
		}

		if ( prevComputedStyle.position === "relative" || prevComputedStyle.position === "static" )
			top  += body.offsetTop,
			left += body.offsetLeft;

		if ( prevComputedStyle.position === "fixed" )
			top  += Math.max(docElem.scrollTop, body.scrollTop),
			left += Math.max(docElem.scrollLeft, body.scrollLeft);

		return { top: top, left: left };
	};

jQuery.offset = {
	initialize: function() {
		if ( this.initialized ) return;
		var body = document.body, container = document.createElement('div'), innerDiv, checkDiv, table, td, rules, prop, bodyMarginTop = body.style.marginTop,
			html = '<div style="position:absolute;top:0;left:0;margin:0;border:5px solid #000;padding:0;width:1px;height:1px;"><div></div></div><table style="position:absolute;top:0;left:0;margin:0;border:5px solid #000;padding:0;width:1px;height:1px;"cellpadding="0"cellspacing="0"><tr><td></td></tr></table>';

		rules = { position: 'absolute', top: 0, left: 0, margin: 0, border: 0, width: '1px', height: '1px', visibility: 'hidden' };
		for ( prop in rules ) container.style[prop] = rules[prop];

		container.innerHTML = html;
		body.insertBefore(container, body.firstChild);
		innerDiv = container.firstChild, checkDiv = innerDiv.firstChild, td = innerDiv.nextSibling.firstChild.firstChild;

		this.doesNotAddBorder = (checkDiv.offsetTop !== 5);
		this.doesAddBorderForTableAndCells = (td.offsetTop === 5);

		innerDiv.style.overflow = 'hidden', innerDiv.style.position = 'relative';
		this.subtractsBorderForOverflowNotVisible = (checkDiv.offsetTop === -5);

		body.style.marginTop = '1px';
		this.doesNotIncludeMarginInBodyOffset = (body.offsetTop === 0);
		body.style.marginTop = bodyMarginTop;

		body.removeChild(container);
		this.initialized = true;
	},

	bodyOffset: function(body) {
		jQuery.offset.initialized || jQuery.offset.initialize();
		var top = body.offsetTop, left = body.offsetLeft;
		if ( jQuery.offset.doesNotIncludeMarginInBodyOffset )
			top  += parseInt( jQuery.curCSS(body, 'marginTop',  true), 10 ) || 0,
			left += parseInt( jQuery.curCSS(body, 'marginLeft', true), 10 ) || 0;
		return { top: top, left: left };
	}
};


jQuery.fn.extend({
	position: function() {
		///	<summary>
		///		Gets the top and left positions of an element relative to its offset parent.
		///	</summary>
		///	<returns type="Object">An object with two integer properties, 'top' and 'left'.</returns>
		var left = 0, top = 0, results;

		if ( this[0] ) {
			// Get *real* offsetParent
			var offsetParent = this.offsetParent(),

			// Get correct offsets
			offset       = this.offset(),
			parentOffset = /^body|html$/i.test(offsetParent[0].tagName) ? { top: 0, left: 0 } : offsetParent.offset();

			// Subtract element margins
			// note: when an element has margin: auto the offsetLeft and marginLeft 
			// are the same in Safari causing offset.left to incorrectly be 0
			offset.top  -= num( this, 'marginTop'  );
			offset.left -= num( this, 'marginLeft' );

			// Add offsetParent borders
			parentOffset.top  += num( offsetParent, 'borderTopWidth'  );
			parentOffset.left += num( offsetParent, 'borderLeftWidth' );

			// Subtract the two offsets
			results = {
				top:  offset.top  - parentOffset.top,
				left: offset.left - parentOffset.left
			};
		}

		return results;
	},

	offsetParent: function() {
		///	<summary>
		///		This method is internal.
		///	</summary>
		///	<private />
		var offsetParent = this[0].offsetParent || document.body;
		while ( offsetParent && (!/^body|html$/i.test(offsetParent.tagName) && jQuery.css(offsetParent, 'position') == 'static') )
			offsetParent = offsetParent.offsetParent;
		return jQuery(offsetParent);
	}
});


// Create scrollLeft and scrollTop methods
jQuery.each( ['Left'], function(i, name) {
	var method = 'scroll' + name;
	
	jQuery.fn[ method ] = function(val) {
		///	<summary>
		///		Gets and optionally sets the scroll left offset of the first matched element.
		///	</summary>
		///	<param name="val" type="Number" integer="true" optional="true">A positive number representing the desired scroll left offset.</param>
		///	<returns type="Number" integer="true">The scroll left offset of the first matched element.</returns>
		if (!this[0]) return null;

		return val !== undefined ?

			// Set the scroll offset
			this.each(function() {
				this == window || this == document ?
					window.scrollTo(
						!i ? val : jQuery(window).scrollLeft(),
						 i ? val : jQuery(window).scrollTop()
					) :
					this[ method ] = val;
			}) :

			// Return the scroll offset
			this[0] == window || this[0] == document ?
				self[ i ? 'pageYOffset' : 'pageXOffset' ] ||
					jQuery.boxModel && document.documentElement[ method ] ||
					document.body[ method ] :
				this[0][ method ];
	};
});

// Create scrollLeft and scrollTop methods
jQuery.each( ['Top'], function(i, name) {
	var method = 'scroll' + name;
	
	jQuery.fn[ method ] = function(val) {
		///	<summary>
		///		Gets and optionally sets the scroll top offset of the first matched element.
		///	</summary>
		///	<param name="val" type="Number" integer="true" optional="true">A positive number representing the desired scroll top offset.</param>
		///	<returns type="Number" integer="true">The scroll top offset of the first matched element.</returns>
		if (!this[0]) return null;

		return val !== undefined ?

			// Set the scroll offset
			this.each(function() {
				this == window || this == document ?
					window.scrollTo(
						!i ? val : jQuery(window).scrollLeft(),
						 i ? val : jQuery(window).scrollTop()
					) :
					this[ method ] = val;
			}) :

			// Return the scroll offset
			this[0] == window || this[0] == document ?
				self[ i ? 'pageYOffset' : 'pageXOffset' ] ||
					jQuery.boxModel && document.documentElement[ method ] ||
					document.body[ method ] :
				this[0][ method ];
	};
});

// Create innerHeight, innerWidth, outerHeight and outerWidth methods
jQuery.each([ "Height" ], function(i, name){

	var tl = i ? "Left"  : "Top",  // top or left
		br = i ? "Right" : "Bottom", // bottom or right
		lower = name.toLowerCase();

	// innerHeight and innerWidth
	jQuery.fn["inner" + name] = function(){
		///	<summary>
		///		Gets the inner height of the first matched element, excluding border but including padding.
		///	</summary>
		///	<returns type="Number" integer="true">The outer height of the first matched element.</returns>
		return this[0] ?
			jQuery.css( this[0], lower, false, "padding" ) :
			null;
	};

	// outerHeight and outerWidth
	jQuery.fn["outer" + name] = function(margin) {
		///	<summary>
		///		Gets the outer height of the first matched element, including border and padding by default.
		///	</summary>
		///	<param name="margins" type="Map">A set of key/value pairs that specify the options for the method.</param>
		///	<returns type="Number" integer="true">The outer height of the first matched element.</returns>
		return this[0] ?
			jQuery.css( this[0], lower, false, margin ? "margin" : "border" ) :
			null;
	};
	
	var type = name.toLowerCase();

	jQuery.fn[ type ] = function( size ) {
		///	<summary>
		///		Set the CSS height of every matched element. If no explicit unit
		///		was specified (like 'em' or '%') then &quot;px&quot; is added to the width.  If no parameter is specified, it gets
		///		the current computed pixel height of the first matched element.
		///		Part of CSS
		///	</summary>
		///	<returns type="jQuery" type="jQuery" />
		///	<param name="cssProperty" type="String">
		///		Set the CSS property to the specified value. Omit to get the value of the first matched element.
		///	</param>

		// Get window width or height
		return this[0] == window ?
			// Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
			document.compatMode == "CSS1Compat" && document.documentElement[ "client" + name ] ||
			document.body[ "client" + name ] :

			// Get document width or height
			this[0] == document ?
				// Either scroll[Width/Height] or offset[Width/Height], whichever is greater
				Math.max(
					document.documentElement["client" + name],
					document.body["scroll" + name], document.documentElement["scroll" + name],
					document.body["offset" + name], document.documentElement["offset" + name]
				) :

				// Get or set width or height on the element
				size === undefined ?
					// Get width or height on the element
					(this.length ? jQuery.css( this[0], type ) : null) :

					// Set the width or height on the element (default to pixels if value is unitless)
					this.css( type, typeof size === "string" ? size : size + "px" );
	};

});

// Create innerHeight, innerWidth, outerHeight and outerWidth methods
jQuery.each([ "Width" ], function(i, name){

	var tl = i ? "Left"  : "Top",  // top or left
		br = i ? "Right" : "Bottom", // bottom or right
		lower = name.toLowerCase();

	// innerHeight and innerWidth
	jQuery.fn["inner" + name] = function(){
		///	<summary>
		///		Gets the inner width of the first matched element, excluding border but including padding.
		///	</summary>
		///	<returns type="Number" integer="true">The outer width of the first matched element.</returns>
		return this[0] ?
			jQuery.css( this[0], lower, false, "padding" ) :
			null;
	};

	// outerHeight and outerWidth
	jQuery.fn["outer" + name] = function(margin) {
		///	<summary>
		///		Gets the outer width of the first matched element, including border and padding by default.
		///	</summary>
		///	<param name="margins" type="Map">A set of key/value pairs that specify the options for the method.</param>
		///	<returns type="Number" integer="true">The outer width of the first matched element.</returns>
		return this[0] ?
			jQuery.css( this[0], lower, false, margin ? "margin" : "border" ) :
			null;
	};
	
	var type = name.toLowerCase();

	jQuery.fn[ type ] = function( size ) {
		///	<summary>
		///		Set the CSS width of every matched element. If no explicit unit
		///		was specified (like 'em' or '%') then &quot;px&quot; is added to the width.  If no parameter is specified, it gets
		///		the current computed pixel width of the first matched element.
		///		Part of CSS
		///	</summary>
		///	<returns type="jQuery" type="jQuery" />
		///	<param name="cssProperty" type="String">
		///		Set the CSS property to the specified value. Omit to get the value of the first matched element.
		///	</param>

		// Get window width or height
		return this[0] == window ?
			// Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
			document.compatMode == "CSS1Compat" && document.documentElement[ "client" + name ] ||
			document.body[ "client" + name ] :

			// Get document width or height
			this[0] == document ?
				// Either scroll[Width/Height] or offset[Width/Height], whichever is greater
				Math.max(
					document.documentElement["client" + name],
					document.body["scroll" + name], document.documentElement["scroll" + name],
					document.body["offset" + name], document.documentElement["offset" + name]
				) :

				// Get or set width or height on the element
				size === undefined ?
					// Get width or height on the element
					(this.length ? jQuery.css( this[0], type ) : null) :

					// Set the width or height on the element (default to pixels if value is unitless)
					this.css( type, typeof size === "string" ? size : size + "px" );
	};

});
})();
