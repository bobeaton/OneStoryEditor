<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <!--doc1 is the original elf document
      doc2 is the fragment that we want to merge into doc1
  <xsl:variable name="doc1" select="document('C:\src\StoryEditor\EditResxLocalization\Properties\Trash.elf')" />
      -->
  <xsl:variable name="doc1" select="document('{0}')" />
  <xsl:variable name="doc2" select="/" />

  <!--set up a variable of all the 'Id's in either doc-->
  <xsl:variable name="doc1IDs" select="$doc1//Entry/@Id" />
  <xsl:variable name="doc2IDs" select="$doc2//Entry/@Id" />
  <xsl:variable name="doc1LangIDs" select="$doc1//Language/@Id" />
  
  <!--switch over to processing doc1 (the non-primary document)-->
  <xsl:template match="EditableLocalization">
    <xsl:copy>
      <!--first apply templates on everything in the doc1-->
      <xsl:apply-templates select="$doc1/EditableLocalization/@*|$doc1/EditableLocalization/*"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Entries">
    <xsl:copy>
      <!--this writes out all the Entries from doc1 (the following template matches
          will will adjust certain things (e.g. if something was changed)-->
      <xsl:apply-templates select="@*|node()" />
      <!--once we finish replicating doc1, we need to add any entries from doc2 that
        aren't in doc1-->
      <xsl:for-each select="$doc2//Entry[not(@Id = $doc1IDs)]">
        <!--copy the Entry element and it's Id-->
        <xsl:copy>
          <xsl:copy-of select="@*"/>
          <!--Call a template to process the new value item(s). For each language
              we put the English value as the value and set a new attribute 
              (needsUpdating) for those that are non-'en'-->
          <xsl:call-template name="CopyNewEntry">
            <xsl:with-param name="newEnValue" select="Value/Text" />
          </xsl:call-template>
        </xsl:copy>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

  <xsl:template name="CopyNewEntry">
    <xsl:param name="newEnValue" />
    <!--for each of the languages in doc1...-->
    <xsl:for-each select="$doc1LangIDs">
      <!--...create a Value element for each of the @lang attribute values-->
      <Value>
        <xsl:attribute name="lang">
          <xsl:value-of select="."/>
        </xsl:attribute>
        <!--for those that *aren't* 'en', add the needsUpdating attribute (so the
            author of that language's localizations knows it needs to be translated)-->
        <xsl:if test=". != 'en'">
          <xsl:attribute name="needsUpdating">
            <xsl:text>true</xsl:text>
          </xsl:attribute>
        </xsl:if>
        <Text>
          <xsl:value-of select="$newEnValue"/>
        </Text>
      </Value>
    </xsl:for-each>
  </xsl:template>
  
  <!--For every 'Entry' element we find (in doc1), see if there's something from 
      from doc2 that we have to overwrite from-->
  <xsl:template match="Entry">
    <xsl:copy>
      <xsl:copy-of select="@*"/>
      <xsl:choose>
        <xsl:when test="@Id = $doc2IDs">
          <!--this means that we have the same item in doc2, so use *it's* value
              rather than ours (it may be updated), but only if it's different-->
          <xsl:variable name="strUpdatedValue" select="$doc2//Entry[@Id = current()/@Id]/Value[@lang = 'en']/Text" />
          <xsl:choose>
            <xsl:when test="Value[@lang = 'en']/Text != $strUpdatedValue">
              <!--this means that the new value is different from the old value. call
                  a template which will update the value and update the other value 
                  elements (of other @langs) to add the 'needsUpdating' attribute so
                  the author of the other language localizations will know it needs
                  to be updated-->
              <xsl:call-template name="UpdateEntryValueEn">
                <xsl:with-param name="strUpdatedValue" select="$strUpdatedValue" />
              </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
              <!--this means that the Entry is in both, but it's not different, so
                  we have to copy over everything else as is-->
              <xsl:apply-templates select="node()" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:when>
        <xsl:otherwise>
          <!--this means that we have an item in doc1 which *isn't* in doc2 two
              we might/should delete it, but let's only mark it with the attribute
              'obsolete'-->
          <xsl:attribute name="obsolete">
            <xsl:text>true</xsl:text>
          </xsl:attribute>
          <xsl:apply-templates select="node()" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:copy>
  </xsl:template>

  <xsl:template name="UpdateEntryValueEn">
    <xsl:param name="strUpdatedValue" />
    <!--iterate over the Value entries (in a doc1//Entry)...-->
    <xsl:for-each select="Value">
      <Value>
        <xsl:copy-of select="@*"/>
        <!--when the language is 'en', it means we have to update the value 
            (from the param)-->
        <xsl:choose>
          <xsl:when test="@lang = 'en'">
            <Text>
              <xsl:value-of select="$strUpdatedValue" />
            </Text>
          </xsl:when>
          <xsl:otherwise>
            <!--otherwise, it means that the 'en' value changed, so any other
                language's value text needs to be updated-->
            <xsl:attribute name="needsUpdating">
              <xsl:text>true</xsl:text>
            </xsl:attribute>
            <xsl:copy-of select="Text"/>
            <PreviousText>
              <xsl:value-of select="preceding-sibling::Value[@lang = 'en']/Text/text()"/>
            </PreviousText>
          </xsl:otherwise>
        </xsl:choose>
      </Value>
    </xsl:for-each>
  </xsl:template>
  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>
