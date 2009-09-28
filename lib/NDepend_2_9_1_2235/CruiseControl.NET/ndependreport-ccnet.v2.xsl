<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">

  <!-- Assumes that you have applied the CCNET Image Handler, explained at http://www.robincurry.org/blog/IntegratingImagesIntoCustomCruiseControlNETBuildReports.aspx -->

  <xsl:output method="html" />

  <xsl:variable name="project" select="//buildproject/text()"/>
  <xsl:variable name="label" select="//buildlabel/text()" />

  <xsl:template match="/">

    <style type="text/css">
      <![CDATA[
                  BODY { font-family: Trebuchet MS; font-size: 10pt; }
                  TD { font-family: Trebuchet MS; font-size: 10pt; }
                  .title { font-size: 25pt; font-weight: bold; }
                  .subtitle { color: #883333; font-size: 20pt; font-weight: bold; background-color: #CCCCCC }
                  .subtitleref { color: blue; font-size: 10pt }
                  .info {color: black; font-size: 10pt}
                  .biginfo {color: black; font-size: 12pt ; font-weight: bold}
                  .infobold {color: black; font-size: 10pt ; font-weight: bold}
                  .hdrcell_left  { color: #FFFFFF ; font-weight: bold; background-color: #B3B3B3; text-align: left;}
                  .hdrcell_leftb  { color: #FFFFFF ; font-weight: bold; background-color: #939393; text-align: left;}
                  .hdrcell_right { color: #FFFFFF ; font-weight: bold; background-color: #B3B3B3; text-align: right;}
                  .hdrcell_rightb { color: #FFFFFF ; font-weight: bold; background-color: #939393; text-align: right;}
                  .datacell_left0 { color: #000055; background-color: #DBDBDB; text-align: left; }
                  .datacell_leftb0{ color: #000055; background-color: #BBBBBB; text-align: left; }
                  .datacell_right0{ color: #000055; background-color: #DBDBDB; text-align: right; }
                  .datacell_rightb0{ color: #000055; background-color: #BBBBBB; text-align: right; }
                  .datacell_red0 { color: #000055; background-color: #FFBBBB; text-align: right; }
                  .datacell_left1 { color: #000055; background-color: #EAEAEA; text-align: left; }
                  .datacell_leftb1 { color: #000055; background-color: #CACACA; text-align: left; }
                  .datacell_right1{ color: #000055; background-color: #EAEAEA; text-align: right; }
                  .datacell_rightb1{ color: #000055; background-color: #CACACA; text-align: right; }
                  .datacell_red1 { color: #000055; background-color: #FFCCCC; text-align: right; }
                  .datacell_stat0 { color: #000055; background-color: #C0C0FF; text-align: right; }
                  .datacell_stat1 { color: #000055; background-color: #D0D0FF; text-align: right; }
                  .datacell_empty { color: #000055; background-color: #FFFFFF; text-align: right; }
                  .cql_ok { color: #000000; background-color: #A0FFA0; text-align: left; font-size: 10pt ; font-weight: bold}
                  .cql_warning { color: #000000; background-color: #FFFF70; text-align: left; font-size: 10pt ; font-weight: bold}
                  .cql_error { color: #FFFFFF; background-color: #FF0000; text-align: left; font-size: 10pt ; font-weight: bold}
		]]>
    </style>
    <div id="NDepend-report">
      <xsl:apply-templates select="//NDepend" />
    </div>
  </xsl:template>

  <xsl:template match="NDepend">
    <div class="title">NDepend Report</div>
    <p></p>
    <xsl:apply-templates select="ReportInfo" />
    <div class="biginfo">
      To get explanations concerning these metrics, go to the <a HREF="http://www.NDepend.com" target="_blank">NDepend home page.</a>
    </div>
    <p></p>
    <p></p>
    <A HREF="#APPMETRIC">
      <div class="subtitleref">Application Metrics</div>
    </A>
    <A HREF="#ASMMETRIC">
      <div class="subtitleref">Assemblies Metrics</div>
    </A>
    <A HREF="#VISUALNDEPENDVIEW">
      <div class="subtitleref">VisualNDepend View</div>
    </A>
    <A HREF="#GRAPH">
      <div class="subtitleref">Assemblies Abstracness vs. Instability</div>
    </A>
    <A HREF="#ASMDEP">
      <div class="subtitleref">Assemblies Dependencies</div>
    </A>
    <A HREF="#ASMDEPDIAG">
      <div class="subtitleref">Assemblies Dependencies Diagram</div>
    </A>
    <A HREF="#ASMBUILDORDER">
      <div class="subtitleref">Assemblies build order</div>
    </A>
    <A HREF="#INFOWARNINGS">
      <div class="subtitleref">NDepend information and warnings</div>
    </A>
    <a HREF="#CQLQUERIESCONSTRAINTS">
      <div class="subtitleref">CQL Queries and Constraints</div>
    </a>
    <A HREF="#TYPEMETRIC">
      <div class="subtitleref">Types Metrics</div>
    </A>
    <!-- A HREF="#TYPEDEP">
			<div class="subtitleref">Types Dependencies</div>
		</A -->
    <p></p>
    <p></p>
    <A NAME="APPMETRIC">
      <div class="subtitle">Application Metrics</div>
    </A>
    <p></p>
    <xsl:apply-templates select="ApplicationMetrics" />
    <p></p>
    <p></p>
    <A NAME="ASMMETRIC">
      <div class="subtitle">Assemblies Metrics</div>
    </A>
    <p></p>
    <xsl:apply-templates select="AssembliesMetrics" />
    <p></p>
    <p></p>
    <A NAME="VISUALNDEPENDVIEW">
      <div class="subtitle">VisualNDepend View</div>
    </A>
    <p></p>
    <img>
      <xsl:attribute name="src">VisualNDependView.png</xsl:attribute>
    </img>
    <p></p>
    <p></p>
    <A NAME="GRAPH">
      <div class="subtitle">Assemblies Abstracness vs. Instability</div>
    </A>
    <p></p>
    <img>
      <xsl:attribute name="src">AbstracnessVSInstability.png</xsl:attribute>
    </img>
    <p></p>
    <p></p>
    <A NAME="ASMDEP">
      <div class="subtitle">Assemblies Dependencies</div>
    </A>
    <p></p>
    <xsl:apply-templates select="AssemblyDependencies" />
    <p></p>
    <p></p>
    <A NAME="ASMDEPDIAG">
      <div class="subtitle">Assemblies Dependencies Diagram</div>
    </A>
    <p></p>
    <div class="info">Blue : Assemblies of your application.</div>
    <div class="info">Yellow : Framework assemblies referenced by assemblies of your application.</div>
    <img>
      <xsl:attribute name="src">ComponentDependenciesDiagram.png</xsl:attribute>
    </img>
    <p></p>
    <p></p>
    <A NAME="ASMBUILDORDER">
      <div class="subtitle">Assemblies build order</div>
    </A>
    <p></p>
    <xsl:choose>
      <xsl:when test="AssemblySortForCompilOrObfusk">
        <xsl:apply-templates select="AssemblySortForCompilOrObfusk" />
      </xsl:when>
      <xsl:otherwise>
        <div class="info">At least one cycle exists in the assemblies dependencies. The build order cannot be computed.</div>
      </xsl:otherwise>
    </xsl:choose>
    <p></p>
    <p></p>
    <A NAME="INFOWARNINGS">
      <div class="subtitle">NDepend information and warnings</div>
    </A>
    <p></p>
    <xsl:apply-templates select="InfoWarnings" />
    <p></p>
    <p></p>
    <A NAME="CQLQUERIESCONSTRAINTS">
      <div class="subtitle">CQL Queries and Constraints</div>
    </A>
    <p></p>
    <xsl:apply-templates select="CQLResult" />
    <p></p>
    <p></p>
    <A NAME="TYPEMETRIC">
      <div class="subtitle">Types Metrics</div>
    </A>
    <p></p>
    <xsl:apply-templates select="TypesMetrics" />
    <p></p>
    <!-- p></p>
		<A NAME="TYPEDEP">
			<div class="subtitle">Types Dependencies</div>
		</A>
		<p></p>
		<xsl:apply-templates select="TypeReferencement" />
		<p></p -->
  </xsl:template>

  <xsl:template match="ReportInfo">
    <div class="biginfo">
      Application name: <font color="#FF0000">
        <xsl:value-of select="@AppName"/>
      </font>
    </div>
    <div class="biginfo">
      This report has been built on: <font color="#FF0000">
        <xsl:value-of select="@NDependDate"/>
      </font>
    </div>
  </xsl:template>

  <xsl:template match="TypeReferencement">
    <table border="1" cellpadding="3" cellspacing="0" bordercolor="white">
      <tr>
        <td class="hdrcell_left">Type</td>
        <td class="hdrcell_left">Uses...</td>
        <td class="hdrcell_left">Is used by...</td>
      </tr>
      <xsl:apply-templates select="Type" />
    </table>
    <p></p>
  </xsl:template>

  <xsl:template match="Type">
    <tr>
      <td class="datacell_left{position() mod 2}">
        <xsl:element name="a">
          <xsl:attribute name="Name">
            <xsl:value-of select="@Name"/>
          </xsl:attribute>
          <xsl:value-of select="@Name"/>
        </xsl:element>
      </td>

      <td class="datacell_left{position() mod 2}">
        <xsl:choose>
          <xsl:when test="Use">
            <xsl:apply-templates select="Use" />
          </xsl:when>
          <xsl:otherwise> - </xsl:otherwise>
        </xsl:choose>
      </td>

      <td class="datacell_left{position() mod 2}">
        <xsl:choose>
          <xsl:when test="UsedBy">
            <xsl:apply-templates select="UsedBy" />
          </xsl:when>
          <xsl:otherwise> - </xsl:otherwise>
        </xsl:choose>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="UsedBy">
    <xsl:for-each select="Name">
      <xsl:element name="a">
        <xsl:attribute name="href">
          #<xsl:apply-templates/>
        </xsl:attribute>
        <xsl:apply-templates/>
      </xsl:element> ;
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="Use">
    <xsl:for-each select="Name">
      <xsl:element name="a">
        <xsl:attribute name="href">
          #<xsl:apply-templates/>
        </xsl:attribute>
        <xsl:apply-templates/>
      </xsl:element> ;
    </xsl:for-each>
  </xsl:template>


  <xsl:template match="InfoWarnings">
    <xsl:for-each select="Info">
      <div class="info">
        <xsl:apply-templates/>
      </div>
    </xsl:for-each>
    <xsl:for-each select="Warning">
      <div class="info">
        WARNING: <xsl:apply-templates/>
      </div>
    </xsl:for-each>
    <xsl:for-each select="Error">
      <div class="info">
        ERROR:  <xsl:apply-templates/>
      </div>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="AssemblySortForCompilOrObfusk">
    <xsl:for-each select="Assembly">
      <div class="info">
        <xsl:value-of select="@Assembly"/>
      </div>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="AssemblyDependencies">
    <table border="1" cellpadding="3" cellspacing="0" bordercolor="white">
      <tr>
        <td class="hdrcell_left">Assembly</td>
        <td class="hdrcell_left">Depends on...</td>
        <td class="hdrcell_left">Is referenced by...</td>
      </tr>
      <xsl:apply-templates select="Dependencies_For" />
    </table>
    <p></p>
  </xsl:template>

  <xsl:template match="Dependencies_For">
    <tr>
      <td class="datacell_left{position() mod 2}">
        <xsl:element name="a">
          <xsl:attribute name="Name">
            <xsl:value-of select="@Assembly"/>
          </xsl:attribute>
          <xsl:value-of select="@Assembly"/>
        </xsl:element>
      </td>

      <td class="datacell_left{position() mod 2}">
        <xsl:choose>
          <xsl:when test="DependsOn">
            <xsl:apply-templates select="DependsOn" />
          </xsl:when>
          <xsl:otherwise> - </xsl:otherwise>
        </xsl:choose>
      </td>

      <td class="datacell_left{position() mod 2}">
        <xsl:choose>
          <xsl:when test="ReferencedBy">
            <xsl:apply-templates select="ReferencedBy" />
          </xsl:when>
          <xsl:otherwise> - </xsl:otherwise>
        </xsl:choose>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="DependsOn">
    <xsl:for-each select="DependsOn_Name">
      <xsl:element name="a">
        <xsl:attribute name="href">
          #<xsl:apply-templates/>
        </xsl:attribute>
        <xsl:apply-templates/>
      </xsl:element> ;
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="ReferencedBy">
    <xsl:for-each select="ReferencedBy_Name">
      <xsl:element name="a">
        <xsl:attribute name="href">
          #<xsl:apply-templates/>
        </xsl:attribute>
        <xsl:apply-templates/>
      </xsl:element> ;
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="ApplicationMetrics">
    <div class="info">
      Number of assemblies: <font color="#FF0000">
        <xsl:value-of select="@NAsm"/>
      </font>
    </div>
    <div class="info">
      Number of types: <font color="#FF0000">
        <xsl:value-of select="@NType"/>
      </font>
    </div>
    <div class="info">
      Number of classes: <font color="#FF0000">
        <xsl:value-of select="@NClass"/>
      </font>
    </div>
    <div class="info">
      Number of abstract classes: <font color="#FF0000">
        <xsl:value-of select="@NAbstractClass"/>
      </font>
    </div>
    <div class="info">
      Number of interfaces: <font color="#FF0000">
        <xsl:value-of select="@NInterface"/>
      </font>
    </div>
    <div class="info">
      Number of value types: <font color="#FF0000">
        <xsl:value-of select="@NValueType"/>
      </font>
    </div>
    <div class="info">
      Number of exception classes: <font color="#FF0000">
        <xsl:value-of select="@NExceptionType"/>
      </font>
    </div>
    <div class="info">
      Number of attribute classes: <font color="#FF0000">
        <xsl:value-of select="@NAttributeType"/>
      </font>
    </div>
    <div class="info">
      Number of delegate classes: <font color="#FF0000">
        <xsl:value-of select="@NDelegateType"/>
      </font>
    </div>
    <div class="info">
      Number of enumerations classes: <font color="#FF0000">
        <xsl:value-of select="@NEnumType"/>
      </font>
    </div>
    <div class="info">
      Number of generic type definitions: <font color="#FF0000">
        <xsl:value-of select="@NGenericTypesDefinition"/>
      </font>
    </div>
    <div class="info">
      Number of generic method definitions: <font color="#FF0000">
        <xsl:value-of select="@NGenericMethodsDefinition"/>
      </font>
    </div>
    <div class="info">
      Number of IL instructions: <font color="#FF0000">
        <xsl:value-of select="@NILInstruction"/>
      </font>
    </div>
    <div class="info">
      Percentage of public types: <font color="#FF0000">
        <xsl:value-of select="@PercentPublicType"/>
      </font>%
    </div>
    <div class="info">
      Percentage of public methods: <font color="#FF0000">
        <xsl:value-of select="@PercentPublicMethod"/>
      </font>%
    </div>
    <div class="info">
      Percentage of classes with at least one public field: <font color="#FF0000">
        <xsl:value-of select="@PercentClassWithAtLeastOnePublicField"/>
      </font>%
    </div>
    <p></p>
    <table border="1" cellpadding="3" cellspacing="0" bordercolor="white">
      <tr>
        <td class="hdrcell_left">Stat</td>
        <td class="hdrcell_left"># Occurrences</td>
        <td class="hdrcell_left">Avg</td>
        <td class="hdrcell_left">StdDev</td>
        <td class="hdrcell_left">Max</td>
      </tr>
      <tr>
        <xsl:apply-templates select="PropertyOnInterface" />
      </tr>
      <tr>
        <xsl:apply-templates select="MethodOnInterface" />
      </tr>
      <tr>
        <xsl:apply-templates select="ArgOnMethodOnInterface" />
      </tr>
      <tr>
        <xsl:apply-templates select="PublicPropertyOnClass" />
      </tr>
      <tr>
        <xsl:apply-templates select="PublicMethodOnClass" />
      </tr>
      <tr>
        <xsl:apply-templates select="ArgOnPublicMethodOnClass" />
      </tr>
      <tr>
        <xsl:apply-templates select="ILInstructionInNonAbstractMethods" />
      </tr>
      <tr>
        <xsl:apply-templates select="ILInstructionInType" />
      </tr>
      <tr>
        <xsl:apply-templates select="ResponseForType" />
      </tr>
      <tr>
        <xsl:apply-templates select="MethodCC" />
      </tr>
    </table>
    <p></p>
    <p></p>
  </xsl:template>


  <xsl:template match="PropertyOnInterface">
    <td class="datacell_left1">
      Properties on Interfaces
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Occ"/> Interfaces
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right1">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> properties on <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="MethodOnInterface">
    <td class="datacell_left0">
      Methods on Interfaces
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Occ"/> Interfaces
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right0">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> methods on <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="ArgOnMethodOnInterface">
    <td class="datacell_left1">
      Arguments on Methods on Interfaces
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Occ"/> Methods
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right1">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> arguments on <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="PublicPropertyOnClass">
    <td class="datacell_left0">
      Public Properties on Classes
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Occ"/> Classes
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right0">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> public properties on <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="PublicMethodOnClass">
    <td class="datacell_left1">
      Public Methods on Classes
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Occ"/> Classes
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right1">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> public methods on <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="ArgOnPublicMethodOnClass">
    <td class="datacell_left0">
      Arguments on Public Methods on Classes
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Occ"/> Methods
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right0">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> arguments on <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>


  <xsl:template match="ILInstructionInNonAbstractMethods">
    <td class="datacell_left1">
      IL Instructions in non-abstract Methods
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Occ"/> Methods
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right1">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> IL instructions in <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="ILInstructionInType">
    <td class="datacell_left0">
      Number of IL Instructions par non-abstract Types
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Occ"/> Types
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right0">
      <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> IL instructions in <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>


  <xsl:template match="MethodCC">
    <td class="datacell_left1">
      Cyclomatic complexity on non abstract Methods
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Occ"/> Methods
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right1">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right1">
      CC = <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> for <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="ResponseForType">
    <td class="datacell_left0">
      Response for Types (IL instructions)
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Occ"/> Types
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@Avg"/>
    </td>
    <td class="datacell_right0">
      <xsl:value-of select="@StdDev"/>
    </td>
    <td class="datacell_right0">
      RFT = <font color="#FF0000">
        <xsl:value-of select="@MaxVal"/>
      </font> IL instructions on <font color="#FF0000">
        <xsl:value-of select="@MaxName"/>
      </font>
    </td>
  </xsl:template>

  <xsl:template match="TypesMetrics">
    <a HREF="http://www.NDepend.com/Metrics.aspx#RFT"    target="_blank">
      <div class="subtitleref">rft: Response For Type</div>
    </a>
    <a HREF="http://www.NDepend.com/Metrics.aspx#LCOM"   target="_blank">
      <div class="subtitleref">lcom: Lack of Cohesion Of Methods of a class</div>
    </a>
    <a HREF="http://www.NDepend.com/Metrics.aspx#ILCC"     target="_blank">
      <div class="subtitleref">CC: Cyclomatic Complexity</div>
    </a>
    <a HREF="http://www.NDepend.com/Metrics.aspx#AfferentCoupling" target="_blank">
      <div class="subtitleref">Ca: Afferent Coupling</div>
    </a>
    <a HREF="http://www.NDepend.com/Metrics.aspx#EfferentCoupling" target="_blank">
      <div class="subtitleref">Ce: Efferent Coupling</div>
    </a>
    <a HREF="http://www.NDepend.com/Metrics.aspx#ABC"    target="_blank">
      <div class="subtitleref">ABC: Association Between Classes</div>
    </a>
    <a HREF="http://www.NDepend.com/Metrics.aspx#NOC"    target="_blank">
      <div class="subtitleref">NOC: Number Of Children</div>
    </a>
    <a HREF="http://www.NDepend.com/Metrics.aspx#DIT"    target="_blank">
      <div class="subtitleref">DIT: Depth in Inheritence Tree</div>
    </a>
    <table border="1" cellpadding="3" cellspacing="0" bordercolor="white">
      <tr>
        <td class="hdrcell_left">Type</td>
        <!--td class="hdrcell_rightb"><a HREF="http://www.NDepend.com/Metrics.aspx#RFT"    target="_blank">rft Method</a></td-->
        <!--td class="hdrcell_rightb"><a HREF="http://www.NDepend.com/Metrics.aspx#RFT"   target="_blank">rft ILInst</a></td-->
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#RANK"      target="_blank">rank</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#LCOM"      target="_blank">lcom</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#LCOM"  target="_blank">lcom(HS)</a>
        </td>
        <td class="hdrcell_rightb">
          <a HREF="http://www.NDepend.com/Metrics.aspx#ILCC" target="_blank">CC</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#AfferentCoupling"     target="_blank">Ca</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#EfferentCoupling"     target="_blank">Ce</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#ABC"     target="_blank">ABC</a>
        </td>
        <td class="hdrcell_rightb"># Instance Methods</td>
        <td class="hdrcell_rightb"># Static Methods</td>
        <td class="hdrcell_rightb"># Prop</td>
        <td class="hdrcell_rightb"># Fld</td>
        <td class="hdrcell_rightb"># ILInst</td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#NOC" target="_blank">NOC</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#DIT" target="_blank">DIT</a>
        </td>

        <td class="hdrcell_leftb">Namespace</td>

      </tr>
      <xsl:apply-templates select="TypeMetric" />
    </table>
    <p></p>
  </xsl:template>

  <xsl:template match="TypeMetric">
    <tr>
      <td class="datacell_left{position() mod 2}">
        <xsl:value-of select="@TypeName"/>
      </td>

      <!--xsl:choose>
				<xsl:when test="@IsBadResponseForAType_Method='True'">
					<td class="datacell_red{position() mod 2}"><xsl:value-of select="@ResponseForAType_Method"/></td>
				</xsl:when>
				<xsl:otherwise>
				    <td class="datacell_rightb{position() mod 2}"><xsl:value-of select="@ResponseForAType_Method"/></td>
				</xsl:otherwise>
           </xsl:choose>
           
           <xsl:choose>
				<xsl:when test="@IsBadResponseForAType_ILInst='True'">
					<td class="datacell_red{position() mod 2}"><xsl:value-of select="@ResponseForAType_PercentageApp"/>% (<xsl:value-of select="@ResponseForAType_ILInst"/>)</td>
				</xsl:when>
				<xsl:otherwise>
				    <td class="datacell_rightb{position() mod 2}"><xsl:value-of select="@ResponseForAType_PercentageApp"/>% (<xsl:value-of select="@ResponseForAType_ILInst"/>)</td>
				</xsl:otherwise>
           </xsl:choose-->


      <xsl:choose>
        <xsl:when test="@IsBadTypeRank='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@TypeRank"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@TypeRank"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadLackOfCohesionOfMethods='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@LackOfCohesionOfMethods"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@LackOfCohesionOfMethods"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadLackOfCohesionOfMethods_HS='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@LackOfCohesionOfMethods_HS"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@LackOfCohesionOfMethods_HS"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadCyclomaticComplexity='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@CyclomaticComplexity"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_rightb{position() mod 2}">
            <xsl:value-of select="@CyclomaticComplexity"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadAfferentCoupling='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@AfferentCoupling"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@AfferentCoupling"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadEfferentCoupling='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@EfferentCoupling"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@EfferentCoupling"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadAssociationBetweenClasses='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@AssociationBetweenClasses"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@AssociationBetweenClasses"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadNInstanceMethods='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@NInstanceMethods"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_rightb{position() mod 2}">
            <xsl:value-of select="@NInstanceMethods"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadNStaticMethods='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@NStaticMethods"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_rightb{position() mod 2}">
            <xsl:value-of select="@NStaticMethods"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadNProperties='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@NProperties"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_rightb{position() mod 2}">
            <xsl:value-of select="@NProperties"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadNFields='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@NFields"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_rightb{position() mod 2}">
            <xsl:value-of select="@NFields"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadNILInstructionOnType='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@NILInstructionOnType"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_rightb{position() mod 2}">
            <xsl:value-of select="@NILInstructionOnType"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadNChildren='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@NChildren"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@NChildren"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:choose>
        <xsl:when test="@IsBadDepthOfInheritanceTree='True'">
          <td class="datacell_red{position() mod 2}">
            <xsl:value-of select="@DepthOfInheritanceTree"/>
          </td>
        </xsl:when>
        <xsl:otherwise>
          <td class="datacell_right{position() mod 2}">
            <xsl:value-of select="@DepthOfInheritanceTree"/>
          </td>
        </xsl:otherwise>
      </xsl:choose>



      <td class="datacell_leftb{position() mod 2}">
        <xsl:value-of select="@TypeNamespace"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="AssembliesMetrics">
    <table border="1" cellpadding="3" cellspacing="0" bordercolor="white">
      <tr>
        <td class="hdrcell_left">Assembly</td>
        <td class="hdrcell_right"># Types</td>
        <td class="hdrcell_right"># Abstract Types</td>
        <td class="hdrcell_right"># IL Inst</td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#AfferentCoupling" target="_blank">Afferent Coupling</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#EfferentCoupling" target="_blank">Efferent Coupling</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#RelationalCohesion" target="_blank">Relational Cohesion</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#Instability" target="_blank">Instability</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#Abstracness" target="_blank">Abstractness</a>
        </td>
        <td class="hdrcell_right">
          <a HREF="http://www.NDepend.com/Metrics.aspx#DitFromMainSeq" target="_blank">Distance</a>
        </td>
      </tr>
      <xsl:apply-templates select="Assembly" />
    </table>
    <p></p>
  </xsl:template>

  <xsl:template match="Assembly">
    <tr>
      <td class="datacell_left{position() mod 2}">
        <xsl:value-of select="@Assembly"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@NTypes"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@NAbstractTypes"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@NILInstructionInAsm"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@AfferentCoupling"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@EfferentCoupling"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@RelationalCohesion"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@Instability"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@Abstractness"/>
      </td>
      <td class="datacell_right{position() mod 2}">
        <xsl:value-of select="@NormDistFrMainSeq"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="CQLResult">
    <xsl:for-each select="Group">
      <xsl:choose>
        <xsl:when test="@Status='Ok'">
          <div class="cql_ok">
            {<xsl:value-of select="@Name"/>}  All CQL queries compile and all CQL contraints are satisfied.
          </div>
        </xsl:when>
        <xsl:when test="@Status='Warn'">
          <div class="cql_warning">
            {<xsl:value-of select="@Name"/>}  Some CQL constraints are not satisfied.
          </div>
        </xsl:when>
        <xsl:when test="@Status='Error'">
          <div class="cql_error">
            {<xsl:value-of select="@Name"/>}  Some CQL queries don't compile.
          </div>
        </xsl:when>
      </xsl:choose>
    </xsl:for-each>

    <xsl:for-each select="Group">
      <xsl:variable name="groupname" select="@Name" />

      <xsl:for-each select="Query">
        <br/>
        <hr/>
        <xsl:choose>
          <xsl:when test="@Status='Warn'">
            <div class="cql_warning">
              WARNING: The following CQL constraint is not satisfied. <xsl:value-of select="@NbNodeMatched"/>  <xsl:value-of select="@KindOfNode"/> on <xsl:value-of select="@NbNodeTested"/> tested match the condition. --> Group {<xsl:value-of select="$groupname"/>}
            </div>
          </xsl:when>
          <xsl:otherwise>
            <div class="cql_error">
              ERROR: The following CQL query doesn't compile. Description: <xsl:value-of select="@ErrorDesc"/> --> Group {<xsl:value-of select="$groupname"/>}
            </div>
          </xsl:otherwise>
        </xsl:choose>
        <pre style="background: #F6F6F6">
          <xsl:value-of select="QueryHtml" disable-output-escaping="yes"/>
        </pre>

        <xsl:choose>
          <!-- and Rows garantee that there is a Rows element-->
          <xsl:when test="@Status='Warn' and Rows">
            <table border="1" cellpadding="3" cellspacing="0" bordercolor="white">
              <tr>
                <td class="hdrcell_left">Name</td>
                <xsl:for-each select="Columns/Column">
                  <td class="hdrcell_left">
                    <xsl:value-of select="." />
                  </td>
                </xsl:for-each>
                <td class="hdrcell_left">Full Name</td>
              </tr>


              <xsl:for-each select="Rows/Row">
                <tr>
                  <!-- We keep track of the position for the nested foreach-->
                  <xsl:variable name="position" select="position()" />
                  <xsl:choose>
                    <xsl:when test="@FullName='Stat'">
                      <!-- Display a stat row-->
                      <td class="datacell_stat{$position mod 2}">
                        <xsl:value-of select="@Name"/>
                      </td>
                      <xsl:for-each select="Val">
                        <td class="datacell_stat{$position mod 2}">
                          <xsl:value-of select="."/>
                        </td>
                      </xsl:for-each>
                      <td class="datacell_empty"></td>
                    </xsl:when>
                    <xsl:otherwise>
                      <!-- Display a normal item row-->
                      <td class="datacell_left{$position mod 2}">
                        <xsl:value-of select="@Name"/>
                      </td>
                      <xsl:for-each select="Val">
                        <td class="datacell_right{$position mod 2}">
                          <xsl:value-of select="."/>
                        </td>
                      </xsl:for-each>
                      <td class="datacell_left{$position mod 2}">
                        <xsl:value-of select="@FullName"/>
                      </td>
                    </xsl:otherwise>
                  </xsl:choose>
                </tr>
              </xsl:for-each>

            </table>
          </xsl:when>
        </xsl:choose>

      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>

