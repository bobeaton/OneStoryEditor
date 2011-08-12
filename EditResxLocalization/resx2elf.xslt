<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:param name="param1"/>
  <xsl:param name="param2"/>
  <xsl:param name="param3"/>

  <xsl:variable name="languageName" select="$param1"/>
  <xsl:variable name="languageId" select="$param2"/>
  <xsl:variable name="fallbackId" select="$param3"/>
  
  <xsl:template match="root">
    <EditableLocalization>
      <Languages>
        <Language>
          <xsl:attribute name="Id">
            <xsl:value-of select="$languageId"/>
          </xsl:attribute>
          <xsl:attribute name="name">
            <xsl:value-of select="$languageName"/>
          </xsl:attribute>
          <xsl:attribute name="fallbackId">
            <xsl:choose>
              <xsl:when test="$fallbackId = ''">
                <xsl:text>en</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="$fallbackId"/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </Language>
      </Languages>
      <Entries>
        <xsl:apply-templates select="node()"/>
      </Entries>
    </EditableLocalization>
  </xsl:template>

  <!--pick off only the string resources-->
  <xsl:template match="data[@xml:space='preserve']">
    <Entry>
      <xsl:attribute name="Id">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <Value>
        <xsl:attribute name="lang">
          <xsl:value-of select="$languageId"/>
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
