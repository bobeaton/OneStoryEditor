<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="root">
    <EditableLocalization>
      <Entries>
        <xsl:apply-templates select="node()"/>
      </Entries>
    </EditableLocalization>
  </xsl:template>

  <!--pick off only the string resources-->
  <xsl:template match="data[not(@type)]">
    <Entry>
      <xsl:attribute name="Id">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <Value>
        <xsl:attribute name="lang">
          <xsl:text>en</xsl:text>
        </xsl:attribute>
        <Text>
          <xsl:value-of select="value/text()"/>
        </Text>
      </Value>
    </Entry>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:apply-templates select="@* | node()"/>
  </xsl:template>

</xsl:stylesheet>
