<?xml version="1.0" encoding="ISO-8859-1"?>

<!--
	$Id: NUnitReport.xsl 102 2006-01-09 18:01:13Z iko $

	This XSL File is based on summary_overview.xsl and toolkit.xsl
	created by Erik Hatcher for Ant's JUnitReport.

	Modified by Tomas Restrepo (tomasr@mvps.org) 
	for use with NUnitReport

	Modified by Ignaz Kohlbecker (Ignaz.Kohlbecker@triamec.com) 
	for use with msbuild
-->

<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:html="http://www.w3.org/Profiles/XHTML-transitional" >

	<xsl:output method="html" indent="yes"/>
	<xsl:param name="project" />
	<xsl:param name="configuration" />
	<xsl:param name="msbuildFilename" />
	<xsl:param name="msbuildBinpath" />
	<xsl:param name="xslFile" />

	<!-- key used to select testcase classnames -->
	<xsl:key name="classnameKey" match="testcase" use="@classname" />
	
	<!--
		format a number in to display its value in percent
		@param value the number to format
	-->
	<xsl:template name="display-time">
		<xsl:param name="value" />
		<xsl:value-of select="format-number($value,'0.000')" />
	</xsl:template>
	
	<!--
		format a number in to display its value in percent
		@param value the number to format
	-->
	<xsl:template name="display-percent">
		<xsl:param name="value"/>
		<xsl:value-of select="format-number($value,'0.00%')"/>
	</xsl:template>
	
	<!--
		transform string like a.b.c to ../../../
		@param path the path to transform into a descending directory path
	-->
	<xsl:template name="path">
		<xsl:param name="path"/>
		<xsl:if test="contains($path,'.')">
			<xsl:text>../</xsl:text>    
			<xsl:call-template name="path">
				<xsl:with-param name="path"><xsl:value-of select="substring-after($path,'.')"/></xsl:with-param>
			</xsl:call-template>    
		</xsl:if>
		<xsl:if test="not(contains($path,'.')) and not($path = '')">
			<xsl:text>../</xsl:text>    
		</xsl:if>   
	</xsl:template>
	
	<!--
		transform string like a.b.c to c
	-->
	<xsl:template name="tail">
		<xsl:param name="name"/>
		<xsl:choose>
			<xsl:when test="contains($name,'.')">
				<xsl:call-template name="tail">
					<xsl:with-param name="name"><xsl:value-of select="substring-after($name,'.')"/></xsl:with-param>
				</xsl:call-template>    
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$name"/>
			</xsl:otherwise>   
		</xsl:choose>
	</xsl:template>
	
	<!--
		template that will convert a carriage return into a br tag
		@param word the text from which to convert CR to BR tag
	-->
	<xsl:template name="br-replace">
		<xsl:param name="word"/>
		<xsl:choose>
			<xsl:when test="contains($word,'&#xA;')">
				<xsl:value-of select="substring-before($word,'&#xA;')"/>
				<br/>
				<xsl:call-template name="br-replace">
					<xsl:with-param name="word" select="substring-after($word,'&#xA;')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$word"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	
	<!-- 
			=====================================================================
			classes summary header
			=====================================================================
	-->
	<xsl:template name="header">
		<xsl:param name="path"/>
		<h1>Project "<xsl:value-of select="$project"/>" - Configuration "<xsl:value-of select="$configuration"/>" - NUnit Test Report</h1>
		<table width="100%">
		<tr>
		   <td align="left">
			  Generated: <xsl:value-of select="@created"/> -
			  <a href="#envinfo">Environment Information</a>
		   </td>
			<td align="right">Designed for use with 
			   <a href='http://www.nunit.org'>NUnit</a> and
			   <a href='http://channel9.msdn.com/wiki/default.aspx/MSBuild.HomePage'>MSBuild</a>.
			</td>
		</tr>
		</table>
		<xsl:text>
		</xsl:text>
		<hr size="1"/>
		<xsl:text>
		</xsl:text>
	</xsl:template>
		
	<!-- 
			=====================================================================
			package summary header
			=====================================================================
	-->
	<xsl:template name="packageSummaryHeader">
		<tr class="TableHeader" valign="top">
			<td><b>Name</b></td>
			<td><b>Time</b></td>
			<td width="5%"><b>Tests</b></td>
			<td width="5%"><b>Errors</b></td>
			<td width="5%"><b>Failures</b></td>
			<td width="5%"><b>Not Run</b></td>
			<td width="10%" nowrap="nowrap"><b>Duration [s]</b></td>
		</tr>
		<xsl:text>
		</xsl:text>
	</xsl:template>
	
	<!-- 
			=====================================================================
			classes summary header
			=====================================================================
	-->
	<xsl:template name="classesSummaryHeader">
		<tr class="TableHeader" valign="top" style="height: 4px">
			<td width="15%"><b>Name</b></td>
			<td width="70%"><b>Description</b></td>
			<td width="10%"><b>Status</b></td>
			<td width="5%" nowrap="nowrap"><b>Duration [s]</b></td>
		</tr>
		<xsl:text>
		</xsl:text>
	</xsl:template>
	
	
	<!-- 
			=====================================================================
			testcase report
			=====================================================================
	-->
	<xsl:template match="test-case">
		<xsl:variable name="result">
			<xsl:choose>
				<xsl:when test="./@executed = 'False'">NotRun</xsl:when>
				<xsl:when test="./failure">Failure</xsl:when>
				<xsl:when test="./error">Error</xsl:when>
				<xsl:otherwise>Pass</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="newid" select="generate-id(@name)" />
		<tr valign="top">
			<xsl:attribute name="class"><xsl:value-of select="$result"/></xsl:attribute>
			<xsl:if test="$result != &quot;Pass&quot;">
				<xsl:attribute name="onclick">javascript:toggle(<xsl:value-of select="$newid"/>)</xsl:attribute>
				<xsl:attribute name="style">cursor: hand;</xsl:attribute>
			</xsl:if>
			<td>
				<xsl:call-template name="tail">
					<xsl:with-param name="name" select="@name"/>
				</xsl:call-template>                
			</td>
			<td><xsl:value-of select="@description"/></td>
			<td><xsl:value-of select="$result"/></td>
			<td align="right">
				<xsl:call-template name="display-time">
					<xsl:with-param name="value" select="@time"/>
				</xsl:call-template>                
			</td>
		</tr>
		<xsl:text>
		</xsl:text>
		<xsl:if test="$result != &quot;Pass&quot;">
			<tr>
				<xsl:attribute name="id">
					<xsl:value-of select="$newid"/>
				</xsl:attribute>
				<td colspan="2" class="FailureDetail">
					<xsl:apply-templates select="./failure"/>
					<xsl:apply-templates select="./reason"/>
					<xsl:apply-templates select="./error"/>
				</td>
			</tr>
			<xsl:text>
			</xsl:text>
		</xsl:if>
	</xsl:template>
	
	<!-- Note : the below template error and failure are the same style
				so just call the same style store in the toolkit template -->
	<xsl:template match="failure">
		<xsl:call-template name="display-failures"/>
	</xsl:template>
	
	<xsl:template match="reason">
		<xsl:call-template name="display-failures"/>
	</xsl:template>
	
	<xsl:template match="error">
		<xsl:call-template name="display-failures"/>
	</xsl:template>
	
	<!-- Style for the error and failure in the tescase template -->
	<xsl:template name="display-failures">
		<code>
			<xsl:choose>
				<xsl:when test="not(./message)">(No message)</xsl:when>
				<xsl:otherwise><xsl:value-of select="./message"/></xsl:otherwise>
			</xsl:choose>
			<!-- display the stacktrace -->
			<br/>
			<xsl:call-template name="br-replace">
				<xsl:with-param name="word" select="./stack-trace"/>
			</xsl:call-template>
		</code>
		<!-- the later is better but might be problematic for non-21" monitors... -->
		<!--pre><xsl:value-of select="."/></pre-->
	</xsl:template>
	
	
	<!-- 
			=====================================================================
			Environtment Info Report
			=====================================================================
	-->
	<xsl:template name="envinfo">
		<a name="envinfo"></a>
		<h2>Environment Information</h2>
		<xsl:text>
		</xsl:text>
		<table border="0" class="DetailTable" width="95%">
			<xsl:text>
			</xsl:text>
			<tr class="EnvInfoHeader">
				<td>Property</td>
				<td>Value</td>
			</tr>
			<xsl:text>
			</xsl:text>
			<tr class="EnvInfoRow">
				<td>MSBuild File</td>
				<td><xsl:value-of select="$msbuildFilename"/></td>
			</tr>
			<xsl:text>
			</xsl:text>
			<tr class="EnvInfoRow">
				<td>MSBuild Bin Path</td>
				<td>
					<xsl:value-of select="$msbuildBinpath"/>
				</td>
			</tr>
			<xsl:text>
			</xsl:text>
			<tr class="EnvInfoRow">
				<td>XSL File</td>
				<td>
					<xsl:value-of select="$xslFile"/>
				</td>
			</tr>
			<xsl:text>
			</xsl:text>
		</table> 
		<a href="#top">Back to top</a>
	</xsl:template>
	
	<!-- I am sure that all nodes are called -->
	<xsl:template match="*">
		<xsl:apply-templates/>
	</xsl:template>
	
	<!--
		====================================================
			Create the page structure
		====================================================
	-->
	<xsl:template match="mergedroot">
		<HTML>
			<HEAD>
				<title><xsl:value-of select="$project"/> Test Report</title>
				
				<!-- put the style in the html so that we can mail it w/o problem -->
				<style type="text/css">
					BODY {
						font: normal 10px verdana, arial, helvetica;
						color:#000000;
					}
					TD {
						FONT-SIZE: 10px
					}
					P {
						line-height:1.5em;
						margin-top:0.5em; margin-bottom:1.0em;
					}
					H1 {
						MARGIN: 0px 0px 5px; 
						FONT: bold arial, verdana, helvetica;
						FONT-SIZE: 16px;
					}
					H2 {
						MARGIN-TOP: 1em; MARGIN-BOTTOM: 0.5em; 
						FONT: bold 14px verdana,arial,helvetica
					}
					H3 {
						MARGIN-BOTTOM: 0.5em; 
						FONT: bold 13px verdana,arial,helvetica;
						color: black;
					}
					H4 {
					   MARGIN-BOTTOM: 0.5em; FONT: bold 100% verdana,arial,helvetica
					}
					H5 {
						MARGIN-BOTTOM: 0.5em; FONT: bold 100% verdana,arial,helvetica
					}
					H6 {
						MARGIN-BOTTOM: 0.5em; FONT: bold 100% verdana,arial,helvetica
					}   
					.Error {
						font-weight:bold; background:#EEEEE0; color:purple;
					}
					.Failure {
						font-weight:bold; background:#EEEEE0; color:red;
					}
					.NotRun {
						font-weight:bold; background:#EEEEE0; color:goldenrod;
					}
					.Pass {
						background:#EEEEE0; 
					}
					.ErrorSum {
						font-weight:bold; 
						background: #ee88aa;
						padding-top: 5px;
					}
					.FailureSum {
						font-weight:bold; 
						background: #ee88aa;
						padding-top: 5px;
					}
					.NotRunSum {
						font-weight:bold; 
						background: #88eeaa;
						padding-top: 5px;
					}
					.PassSum {
						font-weight:bold; 
						background: #88eeaa;
						padding-top: 5px;
					}
					.ClassName {
						font-weight:bold; 
						padding-left: 18px;
						cursor: hand;
						color: #777;
					}
					.TestClassDetails {
						width: 95%;
						margin-bottom: 10px; 
						border-bottom: 1px dotted #999;
					}
					.FailureDetail {
						font-size: -1;
						padding-left: 2.0em;
						border: 1px solid #999;
					}
					.DetailTable TD {
						padding-top: 1px;
						padding-bottom: 1px;
						padding-left: 3px;
						padding-right: 3px;
					}
					.TableHeader {
						background: #6699cc;
						color: white;
						font-weight: bold;
						horizontal-align: center;
					}
					.EnvInfoHeader {
						background: #408080;
						color: white;
						font-weight: bold;
						horizontal-align: center;
					}
					.EnvInfoRow {
						background:#98CBCB
					}
					 
					A:visited {
						color: #0000ff;
					}
					A {
						color: #0000ff;
					}
					A:active {
						color: #800000;
					}
				</style>
				<script language="JavaScript">
					<![CDATA[   
						function toggle (field)
						{
						  if (document.getElementById(field).style.display == "block") {
							document.getElementById(field).style.display = "none";
							document['nav-' + field].src = 
                                'data:image/png;base64,' +
                                'iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAIAAABv85FHAAAABGdBTUEAALGPC/xhBQAAAKJJ' +
                                'REFUGFdj/P//f/+sOgYMUJjWxNA3sxYojQmA4iC5P39+//0LQkDNQPafP7++f/sMlfv+/SsY' +
                                'fQHKffv25eOH108f3oLKff36Ec26+7cvQeU+fXr34cObt2+fA1W8evn48YObt6+dgco9eXjz' +
                                'zo3zNy6fBMpdu3js0pkD50/shspdOrP/2L51h3asOLhjORAd2L7s8K5VIDmg64EUJgKKAwBS' +
                                'fZlqCao5SQAAAABJRU5ErkJggg==';
							document['nav-' + field].alt = "+";
						  } else {
							document.getElementById(field).style.display = "block";
							document['nav-' + field].src = 
                                'data:image/png;base64,' +
                                'iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAIAAABv85FHAAAAAXNSR0IArs4c6QAAAARnQU1B' +
                                'AACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAA' +
                                'AKJJREFUGFdj/P//f/+sOgYMUJjWxNA3sxYojQmA4iC5P39+//0LRUD2nz+/vn/7DJX7/v0r' +
                                'GH0Bom/fvnz88Prpw1tQua9fP6JZd//2Jajcp0/vPnx48/bt8zevn716+fjxg5u3r52Byj15' +
                                'ePPOjfM3Lp+8fun4tYvHLp05cP7EbqjcpTP7j+1bd2jHioM7lgPRge3LDu9aBZIDuh5IYSKg' +
                                'OADf/6RuV97yuQAAAABJRU5ErkJggg==';
							document['nav-' + field].alt = "-";
						  }
						}
					]]> 
				</script>
			</HEAD>
			<body text="#000000" bgColor="#ffffff">
				<a name="#top"></a>
				<xsl:call-template name="header"/>
								
				<!-- Package List part -->
				<xsl:call-template name="packagelist"/>
				<hr size="1" width="95%" align="left"/>
				
				<!-- For each testsuite create the part -->
				<xsl:apply-templates select="test-results">
					<xsl:sort select="@name"/>
				</xsl:apply-templates>
				
				<!-- Environment info part -->
				<xsl:call-template name="envinfo"/>
	
			</body>
		</HTML>
	</xsl:template>
	
	
	
	<!-- ================================================================== -->
	<!-- Write a list of all packages with an hyperlink to the anchor       -->
	<!-- of the package name.                                               -->
	<!-- ================================================================== -->
	<xsl:template name="packagelist">   
		<xsl:variable name="testTotal" select="sum(./test-results/@total)"/>
		<xsl:variable name="errorTotal" select="sum(./test-results/@errors)"/>
		<xsl:variable name="failureTotal" select="sum(./test-results/@failures)"/>
		<xsl:variable name="notRunTotal" select="sum(./test-results/@not-run)"/>
		<xsl:variable name="timeTotal" select="sum(./test-results/test-suite/@time)"/>
		<xsl:variable name="successRate" select="($testTotal - $failureTotal - $errorTotal) div $testTotal"/>

		<h2>Summary</h2>
		<xsl:text>
		</xsl:text>
		
		<table border="0" class="DetailTable" width="95%">
			<xsl:call-template name="packageSummaryHeader"/>
			<xsl:text>
			</xsl:text>

			<!-- list all packages recursively -->
			<xsl:for-each select="./test-results[not(./@name = preceding-sibling::test-suite/@name)]">
				<xsl:sort select="@name"/>
				<xsl:variable name="testCount" select="sum(../test-results[./@name = current()/@name]/@total)"/>
				<xsl:variable name="errorCount" select="sum(../test-results[./@name = current()/@name]/@errors)"/>
				<xsl:variable name="failureCount" select="sum(../test-results[./@name = current()/@name]/@failures)"/>
				<xsl:variable name="notRunCount" select="sum(../test-results[./@name = current()/@name]/@not-run)"/>
				<xsl:variable name="timeCount" select="sum(../test-results/test-suite[./@name = current()/@name]/@time)"/>
				
				<!-- write a summary for the package -->
				<tr valign="top">
					<!-- set a nice color depending if there is an error/failure -->
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="$errorCount &gt; 0">Error</xsl:when>
							<xsl:when test="$failureCount &gt; 0">Failure</xsl:when>
							<xsl:when test="$notRunCount &gt; 0">NotRun</xsl:when>
							<xsl:otherwise>Pass</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>                
					<td><a href="#{generate-id(@name)}"><xsl:value-of select="@name"/></a></td>
					<td><xsl:value-of select="@date"/>, <xsl:value-of select="@time"/></td>
					<td align="right"><xsl:value-of select="$testCount"/></td>
					<td align="right"><xsl:value-of select="$errorCount"/></td>
					<td align="right"><xsl:value-of select="$failureCount"/></td>
					<td align="right"><xsl:value-of select="$notRunCount"/></td>
					<td align="right">
						<xsl:call-template name="display-time">
							<xsl:with-param name="value" select="$timeCount"/>
						</xsl:call-template>                  
					</td>                   
				</tr>
				<xsl:text>
				</xsl:text>
			</xsl:for-each>
			
			<!-- add a row with total sums -->
			<tr valign="top">
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="$errorTotal &gt; 0">ErrorSum</xsl:when>
						<xsl:when test="$failureTotal &gt; 0">FailureSum</xsl:when>
						<xsl:when test="$notRunTotal &gt; 0">NotRunSum</xsl:when>
						<xsl:otherwise>PassSum</xsl:otherwise>
					</xsl:choose>           
				</xsl:attribute>
				<td>Sum</td>
				<td></td>
				<td align="right"><xsl:value-of select="$testTotal"/></td>
				<td align="right"><xsl:value-of select="$errorTotal"/></td>
				<td align="right"><xsl:value-of select="$failureTotal"/></td>
				<td align="right"><xsl:value-of select="$notRunTotal"/></td>
				<td align="right">
					<xsl:call-template name="display-time">
						<xsl:with-param name="value" select="$timeTotal"/>
					</xsl:call-template>
				</td>
			</tr>
			<xsl:text>
			</xsl:text>
			
			<!-- add a row with success rate -->
			<tr valign="top">
				<td colspan="7" align="right">Success Rate:
					<b>
						<xsl:call-template name="display-percent">
							<xsl:with-param name="value" select="$successRate"/>
						</xsl:call-template>
					</b>
				</td>
			</tr>
			<xsl:text>
			</xsl:text>

		</table>        
		<xsl:text>
		</xsl:text>

	</xsl:template>


	<!-- ================================================================== -->
	<!-- Write a list of all classes used in a testsuite, alongside with    -->
	<!-- the results for each one.                                          -->
	<!-- ================================================================== -->
	<xsl:template match="test-results">
		<xsl:variable name="thisResult"><xsl:value-of select="@name"/></xsl:variable>
		<xsl:variable name="results"><xsl:value-of select="generate-id(@name)"/></xsl:variable>
		<xsl:variable name="environment"><xsl:value-of select="$results"/>-env</xsl:variable>

		<xsl:text>
		</xsl:text>
		<!-- create an anchor to this class name -->
		<a name="#{generate-id(@name)}" class="ClassName" >
			<h3>
				<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAIAAABv85FHAAAAAXNSR0IArs4c6QAAAARnQU1B
AACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAA
AKJJREFUGFdj/P//f/+sOgYMUJjWxNA3sxYojQmA4iC5P39+//0LRUD2nz+/vn/7DJX7/v0r
GH0Bom/fvnz88Prpw1tQua9fP6JZd//2Jajcp0/vPnx48/bt8zevn716+fjxg5u3r52Byj15
ePPOjfM3Lp+8fun4tYvHLp05cP7EbqjcpTP7j+1bd2jHioM7lgPRge3LDu9aBZIDuh5IYSKg
OADf/6RuV97yuQAAAABJRU5ErkJggg==" name="nav-{$results}" alt="-" href="javascript:;" onclick="toggle('{$results}')" />
				&#160;TestSuite <xsl:value-of select="@name"/> (<xsl:value-of select="@date"/>, <xsl:value-of select="@time"/>)
			</h3>
		</a>
		<xsl:text>
		</xsl:text>

		<div class="TestClassDetails" id="{$results}" style="display: block">
			<xsl:text>
			</xsl:text>

			<font class="ClassName" >
				<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAIAAABv85FHAAAAAXNSR0IArs4c6QAAAARnQU1B
AACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAA
AKJJREFUGFdj/P//f/+sOgYMUJjWxNA3sxYojQmA4iC5P39+//0LRUD2nz+/vn/7DJX7/v0r
GH0Bom/fvnz88Prpw1tQua9fP6JZd//2Jajcp0/vPnx48/bt8zevn716+fjxg5u3r52Byj15
ePPOjfM3Lp+8fun4tYvHLp05cP7EbqjcpTP7j+1bd2jHioM7lgPRge3LDu9aBZIDuh5IYSKg
OADf/6RuV97yuQAAAABJRU5ErkJggg==" name="nav-{$environment}" alt="-" href="javascript:;" onclick="toggle('{$environment}')" />
				&#160;Test Environment
			</font>
			<xsl:text>
			</xsl:text>

			<div class="TestClassDetails">
				<table border="0" id="{$environment}" style="display: block; margin-left: 35px" class="DetailTable">
					<xsl:text>
					</xsl:text>
					<tr class="EnvInfoHeader">
						<td>Property</td>
						<td>Value</td>
					</tr>
					<xsl:text>
					</xsl:text>
					<tr class="EnvInfoRow">
						<td>NUnit version</td>
						<td>
							<xsl:value-of select="./environment/@nunit-version"/>
						</td>
					</tr>
					<xsl:text>
					</xsl:text>
					<tr class="EnvInfoRow">
						<td>.NET version</td>
						<td>
							<xsl:value-of select="./environment/@clr-version"/>
						</td>
					</tr>
					<xsl:text>
					</xsl:text>
					<tr class="EnvInfoRow">
						<td>OS version</td>
						<td>
							<xsl:value-of select="./environment/@os-version"/>
						</td>
					</tr>
					<xsl:text>
					</xsl:text>
					<tr class="EnvInfoRow">
						<td>Platform</td>
						<td>
							<xsl:value-of select="./environment/@platform"/>
						</td>
					</tr>
					<xsl:text>
					</xsl:text>
					<tr class="EnvInfoRow">
						<td>Working directory</td>
						<td>
							<xsl:value-of select="./environment/@cwd"/>
						</td>
					</tr>
					<xsl:text>
					</xsl:text>
					<tr class="EnvInfoRow">
						<td>Machine/User</td>
						<td>
							<xsl:value-of select="./environment/@machine-name"/>/<xsl:value-of select="./environment/@user"/>
						</td>
					</tr>
					<xsl:text>
					</xsl:text>
				</table>
				<xsl:text>
				</xsl:text>

			</div>
			<xsl:text>
			</xsl:text>

			<xsl:for-each select="descendant::test-suite[count(./results/test-case)>0]">
				<xsl:sort select="@name"/>
				<xsl:variable name="thisClass"><xsl:value-of select="@name"/></xsl:variable>
				<xsl:variable name="details"><xsl:value-of select="generate-id(@name)"/></xsl:variable>
				
				<div class="TestClassDetails">
				<xsl:text>
				</xsl:text>

				<font class="ClassName" >
					<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAkAAAAJCAIAAABv85FHAAAAAXNSR0IArs4c6QAAAARnQU1B
AACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAA
AKJJREFUGFdj/P//f/+sOgYMUJjWxNA3sxYojQmA4iC5P39+//0LRUD2nz+/vn/7DJX7/v0r
GH0Bom/fvnz88Prpw1tQua9fP6JZd//2Jajcp0/vPnx48/bt8zevn716+fjxg5u3r52Byj15
ePPOjfM3Lp+8fun4tYvHLp05cP7EbqjcpTP7j+1bd2jHioM7lgPRge3LDu9aBZIDuh5IYSKg
OADf/6RuV97yuQAAAABJRU5ErkJggg==" name="nav-{$details}" alt="-" href="javascript:;" onclick="toggle('{$details}')" />
					&#160;<xsl:value-of select="$thisClass"/>
				</font>
				<xsl:text>
				</xsl:text>
				
				<table border="0" width="80%" id="{$details}" style="display: block; margin-left: 35px" class="DetailTable">
					<xsl:text>
					</xsl:text>
					<xsl:call-template name="classesSummaryHeader"/>
					<xsl:apply-templates select="./results/test-case">
						<xsl:sort select="@name" />
					</xsl:apply-templates>
					</table>
				</div>
			</xsl:for-each>
			<a href="#top">Back to top</a>
			<hr size="1" width="95%" align="left"/>
			<xsl:text>
			</xsl:text>
		</div>
		<xsl:text>
		</xsl:text>
	</xsl:template>
	

  <xsl:template name="dot-replace">
	  <xsl:param name="package"/>
	  <xsl:choose>
		  <xsl:when test="contains($package,'.')">
		  <xsl:value-of select="substring-before($package,'.')"/>_<xsl:call-template name="dot-replace"><xsl:with-param name="package" select="substring-after($package,'.')"/></xsl:call-template></xsl:when>
		  <xsl:otherwise><xsl:value-of select="$package"/></xsl:otherwise>
	  </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
