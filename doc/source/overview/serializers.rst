Serialization Options
"""""""""""""""""""""

Using NHibernate terminology

+---------------------+------------------+------------------------+------------------------+------------------------+
|                     | Binary (.net)    | XML (.net)             | XML (custom)           | Json                   |
+=====================+==================+========================+========================+========================+
|Property Access      | Only does fields | Needs properties (r/w) | Needs properties (r/w) | Needs properties (r/w) |
|Requires exact class | Yes              | Yes                    | No, can use interfaces | No, can use interfaces |
+---------------------+------------------+------------------------+------------------------+------------------------+
