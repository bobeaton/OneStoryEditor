<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <!--StoryProject@version changed from 1.4 to 1.5-->
  <xsl:template match="StoryProject/@version">
    <xsl:attribute name="version">
      <xsl:text>1.5</xsl:text>
    </xsl:attribute>
  </xsl:template>
  
  <!--Tests was changed to TestsRetellings and an identical TestsTqAnswers-->
  <xsl:template match="Tests">
    <TestsRetellings>
      <xsl:for-each select="Test">
        <TestRetelling>
          <xsl:attribute name="memberID">
            <xsl:value-of select="@memberID"/>
          </xsl:attribute>
        </TestRetelling>
      </xsl:for-each>
    </TestsRetellings>
    <TestsTqAnswers>
      <xsl:for-each select="Test">
        <TestTqAnswer>
          <xsl:attribute name="memberID">
            <xsl:value-of select="@memberID"/>
          </xsl:attribute>
        </TestTqAnswer>
      </xsl:for-each>
    </TestsTqAnswers>
  </xsl:template>

</xsl:stylesheet>
