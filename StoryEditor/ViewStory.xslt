<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <!--Parameters passed in:
        storySet
        story
  -->
  <xsl:param name="storySet"/>
  <xsl:variable name="pathToRunningFolder">
    <xsl:text>{0}\StageTransitions.xml</xsl:text>
  </xsl:variable>

  <xsl:output method="html" indent="yes"/>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
